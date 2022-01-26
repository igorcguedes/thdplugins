using Turbo.Plugins.Default;
using System.Globalization;
using System.Linq;

namespace Turbo.Plugins.SHAKE
{

    public class RatrunsPlugin : BasePlugin, IInGameWorldPainter, IInGameTopPainter
	{
		public WorldDecoratorCollection zNecroDecorator { get; set; }
		public WorldDecoratorCollection MageDecorator { get; set; }
		
		public BuffPainter BuffPainter_LOTD { get; set; }
		public BuffPainter BuffPainter_Mages { get; set; }
		public BuffPainter BuffPainter_MagesCount { get; set; }
		public BuffPainter BuffPainter_Simulacrum { get; set; }
		public BuffPainter BuffPainter_CoE { get; set; }
		public BuffPainter BuffPainter_BorrowedTime { get; set; }
		
        public BuffRuleCalculator RuleCalculator_LOTD { get; private set; }
        public BuffRuleCalculator RuleCalculator_Mages { get; private set; }
        public BuffRuleCalculator RuleCalculator_MagesCount { get; private set; }
		public BuffRuleCalculator RuleCalculator_Simulacrum { get; private set; }
		public BuffRuleCalculator RuleCalculator_CoE { get; private set; }
		public BuffRuleCalculator RuleCalculator_BorrowedTime { get; private set; }

        public RatrunsPlugin()
		{
            Enabled = true;
		}

        public override void Load(IController hud)
        {
            base.Load(hud);
			
			// Buff - Land Of The Dead
			BuffPainter_LOTD = new BuffPainter(Hud, true)
            {
                Opacity = 0.9f,
                ShowTimeLeftNumbers = true,
                ShowTooltips = false,
                TimeLeftFont = Hud.Render.CreateFont("tahoma", 12, 255, 255, 255, 255, true, false, 255, 0, 0, 0, true),
            };
			RuleCalculator_LOTD = new BuffRuleCalculator(Hud);
            RuleCalculator_LOTD.SizeMultiplier = 1.2f;
			RuleCalculator_LOTD.Rules.Add(new BuffRule(465839) { IconIndex = 0, MinimumIconCount = 1, ShowStacks = false, ShowTimeLeft = true });
			
			// Buff - Skeletal Mages
			BuffPainter_Mages = new BuffPainter(Hud, true)
            {
                Opacity = 0.9f,
                ShowTimeLeftNumbers = false,
                ShowTooltips = false,
            };
			RuleCalculator_Mages = new BuffRuleCalculator(Hud);
            RuleCalculator_Mages.SizeMultiplier = 1.2f;
			RuleCalculator_Mages.Rules.Add(new BuffRule(462089) { IconIndex = 5, MinimumIconCount = 0, ShowStacks = false, ShowTimeLeft = false  });
			
			// Buff - Skeletal Mages Stack Count
			// (this buff paints on top of the mage CD buff)
			// (time left spiral indicator doesn't work atm)
			// (time left numbers looks bad, i just disabled it atm)
			BuffPainter_MagesCount = new BuffPainter(Hud, true)
            {
                Opacity = 0,
                ShowTimeLeftNumbers = false,
                ShowTooltips = false,
                StackFont = Hud.Render.CreateFont("tahoma", 11, 255, 0, 255, 0, true, false, 255, 0, 0, 0, true),
				TimeLeftFont = Hud.Render.CreateFont("tahoma", 8, 255, 255, 255, 255, true, false, 255, 0, 0, 0, true),
				TimeLeftClockBrush = Hud.Render.CreateBrush(1, 0, 0, 0, 0),
            };
			RuleCalculator_MagesCount = new BuffRuleCalculator(Hud);
            RuleCalculator_MagesCount.SizeMultiplier = 1.2f;
			RuleCalculator_MagesCount.Rules.Add(new BuffRule(462089) { IconIndex = 6, MinimumIconCount = 0, ShowStacks = true, ShowTimeLeft = true, UseLegendaryItemTexture = Hud.Inventory.GetSnoItem(2276259506) });
			
			// Buff - Simulacrum
			BuffPainter_Simulacrum = new BuffPainter(Hud, true)
            {
                Opacity = 0.9f,
                ShowTimeLeftNumbers = true,
                ShowTooltips = false,
                TimeLeftFont = Hud.Render.CreateFont("tahoma", 8, 255, 255, 255, 255, true, false, 255, 0, 0, 0, true),
            };
			RuleCalculator_Simulacrum = new BuffRuleCalculator(Hud);
            RuleCalculator_Simulacrum.SizeMultiplier = 0.9f;
			RuleCalculator_Simulacrum.Rules.Add(new BuffRule(465350) { IconIndex = 1, MinimumIconCount = 1, ShowStacks = false, ShowTimeLeft = true });
			
			// Buff - CoE
			BuffPainter_CoE = new BuffPainter(Hud, true)
            {
                Opacity = .4f,
                ShowTimeLeftNumbers = false,
                ShowTooltips = false,
                TimeLeftFont = Hud.Render.CreateFont("tahoma", 7, 255, 255, 255, 255, true, false, 255, 0, 0, 0, true),
            };
			RuleCalculator_CoE = new BuffRuleCalculator(Hud);
            RuleCalculator_CoE.SizeMultiplier = 1.2f;
			RuleCalculator_CoE.Rules.Add(new BuffRule(430674) { IconIndex = null, MinimumIconCount = 1, ShowStacks = false, ShowTimeLeft = true });
			
			// Buff - Decrepify - Borrowed Time
			BuffPainter_BorrowedTime = new BuffPainter(Hud, true)
            {
                Opacity = 0.9f,
                ShowTimeLeftNumbers = false,
                ShowTooltips = false,
                StackFont = Hud.Render.CreateFont("tahoma", 9, 255, 255, 255, 255, true, false, 255, 0, 0, 0, true),
            };
			RuleCalculator_BorrowedTime = new BuffRuleCalculator(Hud);
            RuleCalculator_BorrowedTime.SizeMultiplier = 0.9f;
			RuleCalculator_BorrowedTime.Rules.Add(new BuffRule(471738) { IconIndex = 6, MinimumIconCount = 1, ShowStacks = true, ShowTimeLeft = false });
			
			// zNecro Highlight
			zNecroDecorator = new WorldDecoratorCollection
			(
				new GroundCircleDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(255, 30, 255, 30, 12),
                    Radius = 3.8f,
                },
				new GroundCircleDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(150, 30, 255, 30, 0),
                    Radius = 3.8f,
                },
				new MapShapeDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(255, 30, 255, 30, 0),
                    ShadowBrush = Hud.Render.CreateBrush(96, 0, 0, 0, 1),
                    Radius = 8.0f,
                    ShapePainter = new CircleShapePainter(Hud),
                }
            );
			
			MageDecorator = new WorldDecoratorCollection
			(
                /*new GroundCircleDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(70, 0, 255, 153, 10),
                    Radius = 3.2f,
                },
				new GroundCircleDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(45, 0, 255, 153, 0),
                    Radius = 3.3f,
                }*/
			);

        }

        public void PaintWorld(WorldLayer layer)
        {
			if (Hud.Game.IsInTown) return;
			
			var mages = Hud.Game.Actors.Where(a => (a.SnoActor.Sno == ActorSnoEnum._p6_necro_skeletonmage_c) && (a.SummonerAcdDynamicId == Hud.Game.Me.SummonerId));
            foreach (var mage in mages)
				MageDecorator.Paint(layer, mage, mage.FloorCoordinate, null);
			
			var players = Hud.Game.Players.Where(player => !player.IsMe && player.CoordinateKnown && (player.HeadStone == null));
			
			foreach (var player in players)
			{
				// only if player is wearing oculus
				if (!player.Powers.BuffIsActive(402461, 0)) continue;
				
				// players using skill LOTD
				var skill = player.Powers.UsedSkills.Where(x => x.SnoPower.Sno == 465839).FirstOrDefault();

				if (skill != null)
					zNecroDecorator.Paint(layer, player, player.FloorCoordinate, null);
			}
        }
		
		public void PaintTopInGame(ClipState clipState)
        {
            if (Hud.Game.IsInTown) return;
			if (clipState != ClipState.BeforeClip) return;
			
			float buffposX = 0;
			float buffposY = 0;
			
			// Buff - Land Of The Dead
			// (for zNec: only if wearing oculus ring)
			if (Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.OculusRing.Sno))
			{
				RuleCalculator_LOTD.CalculatePaintInfo(Hud.Game.Me);
				if (RuleCalculator_LOTD.PaintInfoList.Count != 0)
				{
					buffposX = Hud.Window.Size.Width * 0f;
					buffposY = Hud.Window.Size.Height * 0.595f;
					
					BuffPainter_LOTD.PaintHorizontalCenter(RuleCalculator_LOTD.PaintInfoList, buffposX, buffposY, Hud.Window.Size.Width, RuleCalculator_LOTD.StandardIconSize, RuleCalculator_LOTD.StandardIconSpacing);
				}
			}
			
			// Buff - Skeletal Mages
			RuleCalculator_Mages.CalculatePaintInfo(Hud.Game.Me);
            if (RuleCalculator_Mages.PaintInfoList.Count != 0)
			{
				RuleCalculator_MagesCount.CalculatePaintInfo(Hud.Game.Me);
				
				buffposX = Hud.Window.Size.Width * 0f;
				buffposY = Hud.Window.Size.Height * 0.595f;
				
				BuffPainter_Mages.PaintHorizontalCenter(RuleCalculator_Mages.PaintInfoList, buffposX, buffposY, Hud.Window.Size.Width, RuleCalculator_Mages.StandardIconSize, RuleCalculator_Mages.StandardIconSpacing);
				BuffPainter_MagesCount.PaintHorizontalCenter(RuleCalculator_MagesCount.PaintInfoList, buffposX, buffposY, Hud.Window.Size.Width, RuleCalculator_MagesCount.StandardIconSize, RuleCalculator_MagesCount.StandardIconSpacing);
			}
			
			// Buff - Simulacrum
			RuleCalculator_Simulacrum.CalculatePaintInfo(Hud.Game.Me);
            if (RuleCalculator_Simulacrum.PaintInfoList.Count != 0)
			{
				buffposX = Hud.Window.Size.Width *  0.029f;
				buffposY = Hud.Window.Size.Height * 0.595f;
				BuffPainter_Simulacrum.PaintHorizontalCenter(RuleCalculator_Simulacrum.PaintInfoList, buffposX, buffposY, Hud.Window.Size.Width, RuleCalculator_Simulacrum.StandardIconSize, RuleCalculator_Simulacrum.StandardIconSpacing);
			}
			
			// Buff - CoE
			RuleCalculator_CoE.CalculatePaintInfo(Hud.Game.Me);
			if (RuleCalculator_CoE.PaintInfoList.Count != 0)
			{
				buffposX = Hud.Window.Size.Width * 0f;
				buffposY = Hud.Window.Size.Height * 0.655f;
				BuffPainter_CoE.PaintHorizontalCenter(RuleCalculator_CoE.PaintInfoList, buffposX, buffposY, Hud.Window.Size.Width, RuleCalculator_CoE.StandardIconSize, RuleCalculator_CoE.StandardIconSpacing);
			}
			
			// Buff - Decrepify - Borrowed Time
			RuleCalculator_BorrowedTime.CalculatePaintInfo(Hud.Game.Me);
			if (RuleCalculator_BorrowedTime.PaintInfoList.Count != 0)
			{
				buffposX = Hud.Window.Size.Width * -0.058f;
				buffposY = Hud.Window.Size.Height * 0.595f;
				BuffPainter_BorrowedTime.PaintHorizontalCenter(RuleCalculator_BorrowedTime.PaintInfoList, buffposX, buffposY, Hud.Window.Size.Width, RuleCalculator_BorrowedTime.StandardIconSize, RuleCalculator_BorrowedTime.StandardIconSpacing);
			}
			
        }

    }

}