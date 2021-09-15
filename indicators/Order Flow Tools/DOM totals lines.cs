using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;


namespace Order_Flow_Tools
{
    public class DOM_totals_lines : Indicator
    {
        [InputParameter("Level count", 10, 1, 9999, 1, 0)]
        public int InputLevelsCount = 10;

        [InputParameter("Custom tick size", 20, 0.0001, 9999, 0.0001, 4)]
        public double InputCustomTicksize = 0.0001;

        [InputParameter("Font size", 30, 1, 30, 1, 0)]
        public int fontSize = 8;

        [InputParameter("Paint rectangle", 40, 0, 1, 1, 0)]
        public bool paintRect = true;

        [InputParameter("X coordinate", 50, 1, 9999, 1, 0)]
        public int x = 775;

        [InputParameter("Y coordinate", 60, 1, 9999, 1, 0)]
        public int y = 20;

        double _askcumulative = 0.0;
        double _bidcumulative = 0.0;

        public DOM_totals_lines()
            : base()
        {
            Name = "DOM_totals_lines";
            Description = "Show the total Bid/Ask from DOM, needs level II data";

            AddLineSeries("Bids cumulative", Color.DarkRed, 2, LineStyle.Solid);
            AddLineSeries("Asks cumulative", Color.DarkGreen, 2, LineStyle.Solid);
         
            SeparateWindow = true;
        }

        protected override void OnInit()
        {
            this.Symbol.NewLevel2 += Symbol_NewLevel2Handler;

        }

        protected override void OnUpdate(UpdateArgs args)
        {
            if (args.Reason == UpdateReason.HistoricalBar)
                return;
            //get current 'order book' snapshot
            //var dom = this.Symbol.DepthOfMarket.GetDepthOfMarketAggregatedCollections();
            var dom = this.Symbol.DepthOfMarket.GetDepthOfMarketAggregatedCollections(new GetLevel2ItemsParameters()
            {
                AggregateMethod = AggregateMethod.ByPriceLVL,
                LevelsCount = this.InputLevelsCount,
                CalculateCumulative = true,
                CustomTickSize = this.InputCustomTicksize
            });

            _askcumulative = 0.0;
            _bidcumulative = 0.0;

            for (int i = 0; i < dom.Asks.Length; i++)
                _askcumulative += dom.Asks[i].Size;

            for (int i = 0; i < dom.Bids.Length; i++)
                _bidcumulative += dom.Bids[i].Size;

            SetValue(_bidcumulative, 0);
            SetValue(_askcumulative, 1);

        }

        private void Symbol_NewLevel2Handler(Symbol symbol, Level2Quote level2, DOMQuote dom)
        {

        }
        protected override void OnClear()
        {
            this.Symbol.NewLevel2 -= Symbol_NewLevel2Handler;
        }

        public override void OnPaintChart(PaintChartEventArgs args)
        {
            if (paintRect)
            {
                Graphics gr = Graphics.FromHdc(args.Hdc);              

                // Create a font
                Font font = new Font("Arial", fontSize);

                Color rectColor = Color.Teal;
                if (_bidcumulative > _askcumulative)
                    rectColor = Color.Red;
                else if (_bidcumulative < _askcumulative)
                    rectColor = Color.Green;
                else
                    rectColor = Color.Teal;
                Pen lpen = new Pen(rectColor, 3);

                Brush brushBids = Brushes.DarkRed;
                Brush brushAsks = Brushes.DarkGreen;
                gr.DrawRectangle(lpen, x, y, 60, 20);
                gr.DrawString(_bidcumulative.ToString(), font, brushBids, x+5, y+3);
                gr.DrawString(_askcumulative.ToString(), font, brushAsks, x+35, y+3);
                
            }

        }
    }
}
