/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

using System;
using System.Collections.Generic;
using System.Linq;

using QuantConnect.Data.Market;
using QuantConnect.Securities.Option;
using QuantConnect.Securities.Positions;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// This algorithm demonstrate how to use OptionStrategies helper class to batch send orders for common strategies.
    /// In this case, the algorithm tests the Naked Call strategy.
    /// </summary>
    public class NakedCallStrategyAlgorithm : OptionStrategyFactoryMethodsBaseAlgorithm
    {
        private OptionStrategy _nakedCall;

        protected override int ExpectedOrdersCount { get; } = 2;

        protected override void TradeStrategy(OptionChain chain)
        {
            var contract = chain
                .OrderBy(x => Math.Abs(chain.Underlying.Price - x.Strike))
                .ThenByDescending(x => x.Expiry)
                .FirstOrDefault();

            if (contract != null)
            {
                _nakedCall = OptionStrategies.NakedCall(_optionSymbol, contract.Strike, contract.Expiry);
                Buy(_nakedCall, 2);
            }
        }

        protected override void AssertStrategyPositionGroup(IPositionGroup positionGroup)
        {
            if (positionGroup.Positions.Count() != 1)
            {
                throw new RegressionTestException($"Expected position group to have 1 position. Actual: {positionGroup.Positions.Count()}");
            }

            var optionPosition = positionGroup.Positions.Single(x => x.Symbol.SecurityType == SecurityType.Option);
            if (optionPosition.Symbol.ID.OptionRight != OptionRight.Call)
            {
                throw new RegressionTestException($"Expected option position to be a call. Actual: {optionPosition.Symbol.ID.OptionRight}");
            }

            var expectedOptionPositionQuantity = -2;

            if (optionPosition.Quantity != expectedOptionPositionQuantity)
            {
                throw new RegressionTestException($@"Expected option position quantity to be {expectedOptionPositionQuantity}. Actual: {optionPosition.Quantity}");
            }
        }

        protected override void LiquidateStrategy()
        {
            // We can liquidate by selling the strategy
            Sell(_nakedCall, 2);
        }

        /// <summary>
        /// This is used by the regression test system to indicate if the open source Lean repository has the required data to run this algorithm.
        /// </summary>
        public override bool CanRunLocally { get; } = true;

        /// <summary>
        /// This is used by the regression test system to indicate which languages this algorithm is written in.
        /// </summary>
        public override List<Language> Languages { get; } = new() { Language.CSharp, Language.Python };

        /// <summary>
        /// Data Points count of all timeslices of algorithm
        /// </summary>
        public override long DataPoints => 2298;

        /// <summary>
        /// Data Points count of the algorithm history
        /// </summary>
        public override int AlgorithmHistoryDataPoints => 0;

        /// <summary>
        /// Final status of the algorithm
        /// </summary>
        public AlgorithmStatus AlgorithmStatus => AlgorithmStatus.Completed;

        /// <summary>
        /// This is used by the regression test system to indicate what the expected statistics are from running the algorithm
        /// </summary>
        public override Dictionary<string, string> ExpectedStatistics => new Dictionary<string, string>
        {
            {"Total Orders", "2"},
            {"Average Win", "0%"},
            {"Average Loss", "0%"},
            {"Compounding Annual Return", "0%"},
            {"Drawdown", "0%"},
            {"Expectancy", "0"},
            {"Start Equity", "1000000"},
            {"End Equity", "999657.4"},
            {"Net Profit", "0%"},
            {"Sharpe Ratio", "0"},
            {"Sortino Ratio", "0"},
            {"Probabilistic Sharpe Ratio", "0%"},
            {"Loss Rate", "0%"},
            {"Win Rate", "0%"},
            {"Profit-Loss Ratio", "0"},
            {"Alpha", "0"},
            {"Beta", "0"},
            {"Annual Standard Deviation", "0"},
            {"Annual Variance", "0"},
            {"Information Ratio", "0"},
            {"Tracking Error", "0"},
            {"Treynor Ratio", "0"},
            {"Total Fees", "$2.60"},
            {"Estimated Strategy Capacity", "$8000.00"},
            {"Lowest Capacity Asset", "GOOCV WBGM92QHIYO6|GOOCV VP83T1ZUHROL"},
            {"Portfolio Turnover", "2.19%"},
            {"Drawdown Recovery", "0"},
            {"OrderListHash", "56c4d8b527d4a8f8d1cc659f8b2d4fc7"}
        };
    }
}
