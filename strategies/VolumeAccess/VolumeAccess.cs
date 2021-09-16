//
// Dev Alejandro Galindo Cháirez. 2021
//
using System;
using System.Collections.Generic;
using TradingPlatform.BusinessLayer;

namespace VolumeAccess
{
    public class VolumeAccess : Strategy
    {
        [InputParameter("Symbol", 0)]
        public Symbol symbol;

        [InputParameter("Account", 1)]
        public Account account;

        [InputParameter("Period", 2)]
        private Period period = Period.MIN1;

        [InputParameter("Bar Shift", 2)]
        private int barShift = 0;


        public HistoricalData hdm;

        double _askVolume = 0.0;
        double _bidVolume = 0.0;
        double _volume = 0.0;
        double _volumeOF = 0.0;
        bool _dataLoaded = false;

        public VolumeAccess()
            : base()
        {
            this.Name = "VolumeAccess";
            this.Description = "use of volume on a strategy";
        }

        protected override void OnCreated()
        {
            base.OnCreated();
        }

        protected override void OnRun()
        {
            if (symbol == null || account == null || symbol.ConnectionId != account.ConnectionId)
            {
                Log("Incorrect input parameters... Symbol or Account are not specified or they have diffent connectionID.", StrategyLoggingLevel.Error);
                return;
            }

            this.hdm = this.symbol.GetHistory(period, this.symbol.HistoryType, DateTime.UtcNow.AddDays(-100));

            symbol.NewQuote += OnNewQuote;
            symbol.NewLast += OnNewLast;

            IVolumeAnalysisCalculationProgress progress = Core.Instance.VolumeAnalysis.CalculateProfile(this.hdm, new VolumeAnalysisCalculationParameters()
            {
                CalculatePriceLevels = true,
                DeltaCalculationType = this.symbol.DeltaCalculationType
            });
            progress.StateChanged += this.Progress_StateChanged;

            Log("OnRun executed", StrategyLoggingLevel.Info);

        }

        protected override void OnStop()
        {
            base.OnStop();

            if (this.symbol != null)
            {
                symbol.NewQuote -= OnNewQuote;
                symbol.NewLast -= OnNewLast;
            }

            Log("OnStop executed", StrategyLoggingLevel.Info);

        }

        protected override void OnRemove()
        {
            base.OnRemove();
        }

        protected override List<StrategyMetric> OnGetMetrics()
        {
            List<StrategyMetric> result = base.OnGetMetrics();

            // An example of adding custom strategy metrics:
            result.Add("Volume", _volume.ToString());
            result.Add("Current Ask volume", _askVolume.ToString());
            result.Add("Current Bid volume", _bidVolume.ToString());
            result.Add("Volume OF", _volumeOF.ToString());

            return result;
        }

        private void Progress_StateChanged(object sender, VolumeAnalysisTaskEventArgs a)
        {
            if (a.CalculationState == VolumeAnalysisCalculationState.Finished)
                _dataLoaded = true;
        }
        private void OnNewLast(Symbol symbol, Last last)
        {
            StrategyProcess(last);
            //Log("OnNewLast executed", StrategyLoggingLevel.Info);
        }

        private void OnNewQuote(Symbol symbol, Quote quote)
        {
            StrategyProcess(quote);

            _volume = ((HistoryItemBar)hdm[barShift]).Volume;
            _askVolume = ((HistoryItemBar)hdm[barShift]).VolumeAnalysisData.Total.SellVolume;
            _bidVolume = ((HistoryItemBar)hdm[barShift]).VolumeAnalysisData.Total.BuyVolume;
            _volumeOF = ((HistoryItemBar)hdm[barShift]).VolumeAnalysisData.Total.Volume;

            //if (_dataLoaded)
            //{
            //    _askVolume = hdm[0, SeekOriginHistory.End].VolumeAnalysisData.Total.SellVolume;
            //    _bidVolume = hdm[0, SeekOriginHistory.End].VolumeAnalysisData.Total.BuyVolume;
            //}

            //Log("OnNewQuote executed", StrategyLoggingLevel.Info);
        }


        private void StrategyProcess(MessageQuote message)
        {
            //Log("StrategyProcess executed", StrategyLoggingLevel.Info);
        }

        private void QuoteHandler(Symbol instrument, Quote quote)
        {
            //Log("QuoteHandler executed", StrategyLoggingLevel.Info);
        }
    }
}