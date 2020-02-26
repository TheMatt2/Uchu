using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Behaviors;

namespace Uchu.World
{
    public class SkillComponent : ReplicaComponent
    {
        private readonly Dictionary<BehaviorSlot, uint> _activeBehaviors = new Dictionary<BehaviorSlot, uint>();
        
        private readonly Dictionary<uint, ExecutionContext> _handledSkills = new Dictionary<uint, ExecutionContext>();
        
        // This number is taken from testing and is not concrete.
        public const float TargetRange = 11.6f;

        private uint BehaviorSyncIndex { get; set; }
        
        public uint[] DefaultSkillSet { get; set; }

        public Lot SelectedConsumeable { get; set; }

        public uint SelectedSkill
        {
            get => _activeBehaviors[BehaviorSlot.Primary];
            set => _activeBehaviors[BehaviorSlot.Primary] = value;
        }
        
        public override ComponentId Id => ComponentId.SkillComponent;

        protected SkillComponent()
        {
            Listen(OnStart, async () =>
            {
                await using var cdClient = new CdClientContext();

                var skills = await cdClient.ObjectSkillsTable.Where(
                    s => s.ObjectTemplate == GameObject.Lot
                ).ToArrayAsync();

                DefaultSkillSet = skills
                    .Where(s => s.SkillID != default)
                    .Select(s => (uint) s.SkillID)
                    .ToArray();
                
                if (!(GameObject is Player)) return;

                _activeBehaviors[BehaviorSlot.Primary] = 1;
            });
        }
        
        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(false);
        }

        public override void Serialize(BitWriter writer)
        {
        }

        public async Task MountItemAsync(Lot item)
        {
            if (item == default) return;
            
            await using var ctx = new CdClientContext();

            var itemInfo = await ctx.ItemComponentTable.FirstOrDefaultAsync(
                i => i.Id == item.GetComponentId(ComponentId.ItemComponent)
            );
            
            if (itemInfo == default) return;

            var slot = ((ItemType) (itemInfo.ItemType ?? 0)).GetBehaviorSlot();

            RemoveSkill(slot);
            
            await MountSkill(item);

            await EquipSkill(item);
        }

        public async Task DismountItemAsync(Lot item)
        {
            if (item == default) return;

            await using var ctx = new CdClientContext();

            var itemInfo = await ctx.ItemComponentTable.FirstOrDefaultAsync(
                i => i.Id == item.GetComponentId(ComponentId.ItemComponent)
            );
            
            if (itemInfo == default) return;

            var slot = ((ItemType) (itemInfo.ItemType ?? 0)).GetBehaviorSlot();
            
            RemoveSkill(slot);

            await DismountSkill(item);
        }

        private async Task EquipSkill(Lot item)
        {
            if (item == default) return;
            
            await using var ctx = new CdClientContext();

            var itemInfo = await ctx.ItemComponentTable.FirstOrDefaultAsync(
                i => i.Id == item.GetComponentId(ComponentId.ItemComponent)
            );
            
            if (itemInfo == default) return;

            var slot = ((ItemType) (itemInfo.ItemType ?? 0)).GetBehaviorSlot();
            
            var infos = await BehaviorTree.GetSkillsForObject(item);

            var onUse = infos.FirstOrDefault(i => i.CastType == SkillCastType.OnUse);

            if (onUse == default) return;
            
            As<Player>().SendChatMessage($"Adding skill: {onUse.SkillId}");
            
            RemoveSkill(slot);
            
            SetSkill(slot, (uint) onUse.SkillId);
        }

        private async Task MountSkill(Lot item)
        {
            if (item == default) return;
            
            var infos = await BehaviorTree.GetSkillsForObject(item);
            
            var onEquip = infos.FirstOrDefault(i => i.CastType == SkillCastType.OnEquip);

            if (onEquip == default) return;
            
            As<Player>().SendChatMessage($"Mount skill: {onEquip.SkillId}");
            
            var tree = new BehaviorTree(item);

            await tree.BuildAsync();

            await tree.MountAsync(GameObject);
        }

        private async Task DismountSkill(Lot item)
        {
            if (item == default) return;
            
            var infos = await BehaviorTree.GetSkillsForObject(item);

            var onEquip = infos.FirstOrDefault(i => i.CastType == SkillCastType.OnEquip);

            if (onEquip == default) return;
            
            As<Player>().SendChatMessage($"Dismount skill: {onEquip.SkillId}");
            
            var tree = new BehaviorTree(item);

            await tree.BuildAsync();

            await tree.DismantleAsync(GameObject);
        }

        public async Task<float> CalculateSkillAsync(int skillId)
        {
            var stream = new MemoryStream();
            using var writer = new BitWriter(stream, leaveOpen: true);

            var tree = new BehaviorTree(skillId);

            await tree.BuildAsync();

            var syncId = ClaimSyncId();

            var context = await tree.CalculateAsync(GameObject, writer, skillId, syncId, Transform.Position);

            if (!context.FoundTarget) return 0;

            foreach (var player in Zone.Players)
            {
                player.SendChatMessage($"Start: [{syncId}]");
            }
            
            Zone.BroadcastMessage(new EchoStartSkillMessage
            {
                Associate = GameObject,
                CastType = 0,
                Content = stream.ToArray(),
                SkillId = skillId,
                SkillHandle = syncId,
                OptionalOriginator = GameObject,
                OriginatorRotation = GameObject.Transform.Rotation
            });

            return context.SkillTime;
        }

        public async Task StartUserSkillAsync(StartSkillMessage message)
        {
            if (As<Player>() == null) return;

            As<Player>().SendChatMessage($"TARGET: {message.OptionalTarget} [{message.UsedMouse}]");

            try
            {
                if (message.OptionalTarget != null)
                {
                    // There should be more to this
                    if (!message.OptionalTarget.GetComponent<DestructibleComponent>()?.Alive ?? true)
                        message.OptionalTarget = null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                
                return;
            }

            var stream = new MemoryStream(message.Content);
            using (var reader = new BitReader(stream, leaveOpen: true))
            {
                As<Player>().SendChatMessage($"START: {message.SkillId} [{message.Content.Length}]");
                
                var tree = new BehaviorTree(message.SkillId);

                await tree.BuildAsync();

                await using var writeStream = new MemoryStream();
                using var writer = new BitWriter(writeStream);

                var context = await tree.ExecuteAsync(
                    GameObject,
                    reader,
                    writer,
                    SkillCastType.OnUse,
                    message.OptionalTarget
                );

                _handledSkills[message.SkillHandle] = context;
                
                await using var cdClient = new CdClientContext();

                var skillBehavior = await cdClient.SkillBehaviorTable.FirstOrDefaultAsync(
                    s => s.SkillID == message.SkillId
                );

                var cost = skillBehavior.Imaginationcost ?? 0;

                if (GameObject.TryGetComponent<Stats>(out var stats))
                {
                    stats.Imagination = (uint) ((int) stats.Imagination - cost);
                }
                
                Zone.ExcludingMessage(new EchoStartSkillMessage
                {
                    Associate = GameObject,
                    CasterLatency = message.CasterLatency,
                    CastType = message.CastType,
                    Content = message.Content,
                    LastClickedPosition = message.LastClickedPosition,
                    OptionalOriginator = message.OptionalOriginator,
                    OptionalTarget = message.OptionalTarget,
                    OriginatorRotation = message.OriginatorRotation,
                    SkillHandle = message.SkillHandle,
                    SkillId = message.SkillId,
                    UsedMouse = message.UsedMouse
                }, As<Player>());
            }

            await GameObject.GetComponent<MissionInventoryComponent>().UseSkillAsync(
                message.SkillId
            );
        }

        public async Task SyncUserSkillAsync(SyncSkillMessage message)
        {
            var stream = new MemoryStream(message.Content);
            using var reader = new BitReader(stream, leaveOpen: true);

            await using var writeStream = new MemoryStream();
            using var writer = new BitWriter(writeStream);

            var found = _handledSkills.TryGetValue(message.SkillHandle, out var behavior);
            
            As<Player>().SendChatMessage($"SYNC: {message.SkillHandle} [{message.BehaviorHandle}] ; {found}");

            if (found)
            {
                await behavior.SyncAsync(message.BehaviorHandle, reader, writer);
            }

            if (message.Done)
            {
                //_handledSkills.Remove(message.SkillHandle);
            }

            Zone.ExcludingMessage(new EchoSyncSkillMessage
            {
                Associate = GameObject,
                BehaviorHandle = message.BehaviorHandle,
                Content = message.Content,
                Done = message.Done,
                SkillHandle = message.SkillHandle
            }, As<Player>());
        }

        public void SetSkill(BehaviorSlot slot, uint skillId)
        {
            if (_activeBehaviors.TryGetValue(slot, out var currentSkill))
            {
                _activeBehaviors.Remove(slot);
                
                Logger.Information($"Removed skill: [{slot}]: {currentSkill}");
                
                As<Player>().Message(new RemoveSkillMessage
                {
                    Associate = GameObject,
                    SkillId = currentSkill
                });
            }

            _activeBehaviors[slot] = skillId;

            Logger.Information($"Selected skill: [{slot}]: {skillId}");
            
            As<Player>().Message(new AddSkillMessage
            {
                Associate = GameObject,
                CastType = SkillCastType.OnUse,
                SlotId = slot,
                SkillId = skillId
            });
        }
        
        public void RemoveSkill(BehaviorSlot slot)
        {
            if (_activeBehaviors.TryGetValue(slot, out var currentSkill))
            {
                _activeBehaviors.Remove(slot);
                
                Logger.Information($"Removed skill: [{slot}]: {currentSkill}");
                
                As<Player>().Message(new RemoveSkillMessage
                {
                    Associate = GameObject,
                    SkillId = currentSkill
                });
            }

            //
            // Get default skill
            //
            
            var skillId = slot switch
            {
                BehaviorSlot.Invalid => 0u,
                BehaviorSlot.None => 0u,
                BehaviorSlot.Head => 0u,
                BehaviorSlot.LeftHand => 0u,
                BehaviorSlot.Consumeable => 0u,
                BehaviorSlot.Neck => 0u,
                BehaviorSlot.Primary => 1u,
                _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
            };

            _activeBehaviors[slot] = skillId;
            
            Logger.Information($"Selected default skill: [{slot}]: {skillId}");
            
            As<Player>().Message(new AddSkillMessage
            {
                Associate = GameObject,
                CastType = SkillCastType.OnUse,
                SlotId = slot,
                SkillId = skillId
            });
        }

        public uint ClaimSyncId()
        {
            lock (this)
            {
                return ++BehaviorSyncIndex;
            }
        }
    }
}