using Turbo.Plugins.Default;
using System.Linq;
using System.Collections.Generic;

namespace Turbo.Plugins.SHAKE
{
    public class MonkSanc: BasePlugin, IInGameWorldPainter, ICustomizer
    {
		public WorldDecoratorCollection InnerSanctuaryDefaultDecorator{ get; set; }
		public WorldDecoratorCollection InnerSanctuaryInSide { get; set; }	
		public WorldDecoratorCollection InnerSanctuaryInterveneDecorator { get; set; }
		public WorldDecoratorCollection InnerSanctuaryForbiddenDecorator { get; set; }
		
		public Dictionary<ActorSnoEnum,bool> InnerSancSno { get; set; } = new Dictionary<ActorSnoEnum,bool>
		{
			{ActorSnoEnum._x1_monk_innersanctuary_proxy,true}, 				// { x.SnoActor.Sno , ninguno dibujado }
			{ActorSnoEnum._monk_innersanctuaryrune_duration_proxy,true},   
			{ActorSnoEnum._x1_monk_innersanctuaryrune_healing_proxy,true},
			{ActorSnoEnum._x1_monk_innersanctuaryrune_protect_proxy,true},
			{ActorSnoEnum._x1_monk_innersanctuaryrune_intervene_proxy,true},
			{ActorSnoEnum._x1_monk_innersanctuaryrune_forbidden_proxy,true}
		};
		
		public float CircleRadius { get; set; }
		public float InsideRadius { get; set; }
		public float BuffRadius { get; set; }
			
		public MonkSanc()
		{
			Enabled = true;			
		}
		
		public override void Load(IController hud)
		{
			base.Load(hud);
			Order = 30001;
			
			CircleRadius = 13f; // Recommended values: 10f (real size sanc) , 13f (+hitbox)
			InsideRadius = 13f; // Recommended values:  equal to CircleRadius
		
			BuffRadius = 13.5f;   //  Detect buff
		}

        public void Customize()
        {
			InitDecoratorsCircle();
			InitDecoratorsInside();			
		}
			
		private void InitDecoratorsCircle()
		{
			Hud.GetPlugin<PlayerSkillPlugin>().InnerSanctuaryDefaultDecorator				= new WorldDecoratorCollection(); 
			InnerSanctuaryDefaultDecorator													= CreateInnerSanctuaryWorldDecorator(100, 255, 255, 255, 6f);
			Hud.GetPlugin<PlayerSkillPlugin>().InnerSanctuarySanctifiedGroundDecorator		= CreateInnerSanctuaryWorldDecorator(100, 230, 170, 255, 8f);
			Hud.GetPlugin<PlayerSkillPlugin>().InnerSanctuarySafeHavenDecorator				= CreateInnerSanctuaryWorldDecorator(100, 255,  30,  30, 6f);
			Hud.GetPlugin<PlayerSkillPlugin>().InnerSanctuaryTempleOfProtecteionDecorator	= CreateInnerSanctuaryWorldDecorator(255,  255, 0, 0, 9f);
			InnerSanctuaryInterveneDecorator												= CreateInnerSanctuaryWorldDecorator(100, 255, 204,   0, 6f);
			InnerSanctuaryForbiddenDecorator												= CreateInnerSanctuaryWorldDecorator(100, 255, 155,  50, 6f);
		}
		private void InitDecoratorsInside()
        {
			InnerSanctuaryInSide = new WorldDecoratorCollection(
				new GroundCircleDecorator(Hud)
				{
					Brush = Hud.Render.CreateBrush(80, 0, 0, 100, 4, SharpDX.Direct2D1.DashStyle.Dash),
					Radius = InsideRadius,
				}
			);
		}
		
		public WorldDecoratorCollection CreateInnerSanctuaryWorldDecorator(int o,int r,int g,int b, float duration) {
			return new WorldDecoratorCollection(
				new GroundCircleDecorator(Hud)
				{
					Brush = Hud.Render.CreateBrush(245, 51, 255, 51, 4f),
					Radius = 13.0f,
				},
				new GroundLabelDecorator(Hud)
				{
					CountDownFrom = 6,
					TextFont = Hud.Render.CreateFont("tahoma", 9, 255, 100, 255, 150, true, false, 128, 0, 0, 0, true),
				},
				new GroundTimerDecorator(Hud)
				{
					CountDownFrom = duration,
					BackgroundBrushEmpty = Hud.Render.CreateBrush(128, 0, 0, 0, 0),
					BackgroundBrushFill = Hud.Render.CreateBrush(o, r, g, b, 0),
					Radius = 35,
				}	
			);
		}
			
		public void PaintWorld(WorldLayer layer)
		{
			if (layer != WorldLayer.Ground) return;
			var actors = Hud.Game.Actors.Where(x => InnerSancSno.ContainsKey(x.SnoActor.Sno)).OrderBy(d => d.CentralXyDistanceToMe);
			if (actors.Any()) 
			{
				foreach (var actor in actors) 
				{
					bool buff = false;
					switch (actor.SnoActor.Sno)
					{	// ISnoPower Monk_InnerSanctuary { get; } // 317076 ,  if (Hud.Game.Me.Powers.BuffIsActive(317076, 1))  --->  Estamos dentro de algún santuario.
						case ActorSnoEnum._x1_monk_innersanctuary_proxy:				//Default
								InnerSanctuaryDefaultDecorator.Paint(layer, actor, actor.FloorCoordinate, null);
								if ( Hud.Game.Me.Powers.BuffIsActive(317076, 1) && (actor.NormalizedXyDistanceToMe <= BuffRadius)) buff = true;
								
							break;
						case ActorSnoEnum._monk_innersanctuaryrune_duration_proxy:		//Sanctified
								if ( Hud.Game.Me.Powers.BuffIsActive(317076, 1) && (actor.NormalizedXyDistanceToMe <= BuffRadius)) buff = true;
							break;
						case ActorSnoEnum._x1_monk_innersanctuaryrune_healing_proxy:	//SafeHaven
								if (Hud.Game.Me.Powers.BuffIsActive(317076, 3)) buff = true;   // Sólo en éste he podido encontrar como mirar el bufo concreto. ¿Que pasa con el resto?
							break;
						case ActorSnoEnum._x1_monk_innersanctuaryrune_protect_proxy:	//TempleOfProtecteion
								if ( Hud.Game.Me.Powers.BuffIsActive(317076, 1) && (actor.NormalizedXyDistanceToMe <= BuffRadius)) buff = true;
							break;
						case ActorSnoEnum._x1_monk_innersanctuaryrune_intervene_proxy:	//Intervenido
								InnerSanctuaryInterveneDecorator.Paint(layer, actor, actor.FloorCoordinate, null);
								if ( Hud.Game.Me.Powers.BuffIsActive(317076, 1) && (actor.NormalizedXyDistanceToMe <= BuffRadius)) buff = true;
							break;
						case ActorSnoEnum._x1_monk_innersanctuaryrune_forbidden_proxy:	//Palacio Prohibido
								InnerSanctuaryForbiddenDecorator.Paint(layer, actor, actor.FloorCoordinate, null);
								if ( Hud.Game.Me.Powers.BuffIsActive(317076, 1) && (actor.NormalizedXyDistanceToMe <= BuffRadius)) buff = true;
							break;	
					}
					//if (buff && (InnerSancSno[actor.SnoActor.Sno])) 
					{
						//InnerSanctuaryInSide.Paint(layer, actor, actor.FloorCoordinate, null);
						//InnerSancSno[actor.SnoActor.Sno] = false; 
					}			
				}
				foreach(var i in InnerSancSno.Keys.ToList()) InnerSancSno[i] = true;
			}			
		}
	}
}