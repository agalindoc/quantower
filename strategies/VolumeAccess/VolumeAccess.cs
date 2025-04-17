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
        double _delta = 0.0;
        bool _dataLoaded = false;
        DateTime _dateTimeLeft;
        DateTime _dateTimeRight;
        int _count = 0;


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

            //if (_dataLoaded)
            //{
            // An example of adding custom strategy metrics:
            result.Add("Volume", _volume.ToString());
            result.Add("Current Ask volume", _askVolume.ToString());
            result.Add("Current Bid volume", _bidVolume.ToString());
            result.Add("Volume OF", _volumeOF.ToString());
            result.Add("Count", _count.ToString());
            result.Add("Time left:", _dateTimeLeft.ToString());
            result.Add("Time right:", _dateTimeRight.ToString());
            result.Add("Delta", _delta.ToString());
            //}



            return result;
        }

        
        private void OnNewLast(Symbol symbol, Last last)
        {
            StrategyProcess(last);
            //Log("OnNewLast executed", StrategyLoggingLevel.Info);
        }

        public void VolumeAnalysisData_Loaded()
        {

        }

        private void Progress_StateChanged(object sender, VolumeAnalysisTaskEventArgs a)
        {

            if (a.CalculationState == VolumeAnalysisCalculationState.Finished)
            {
                this._dataLoaded = true;

                Log("History Loaded", StrategyLoggingLevel.Info);
            }

        }


        private void OnNewQuote(Symbol symbol, Quote quote)
        {
            Log($"State: {this.hdm.VolumeAnalysisCalculationProgress.State.ToString()}");

            if (this.hdm.VolumeAnalysisCalculationProgress == null)
                Log("Progress is null");

            if (this.hdm.VolumeAnalysisCalculationProgress == null || this.hdm.VolumeAnalysisCalculationProgress.State != VolumeAnalysisCalculationState.Finished)
                return;

            StrategyProcess(quote);

            _volume = ((HistoryItemBar)hdm[barShift]).Volume;
            _askVolume = ((HistoryItemBar)hdm[barShift]).VolumeAnalysisData.Total.SellVolume;
            _bidVolume = ((HistoryItemBar)hdm[barShift]).VolumeAnalysisData.Total.BuyVolume;
            _volumeOF = ((HistoryItemBar)hdm[barShift]).VolumeAnalysisData.Total.Volume;
            _dateTimeLeft = ((HistoryItemBar)hdm[barShift]).VolumeAnalysisData.TimeLeft;
            _delta = ((HistoryItemBar)hdm[barShift]).VolumeAnalysisData.Total.Delta;
            _dateTimeRight = hdm.FromTime;
            _count = hdm.Count;
            if (double.IsNaN(_askVolume))
                Log("_askVolume is NaN");

            //if (_dataLoaded)
            //{
            //
            // Execute here yor strategy based on volume data
            //
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
