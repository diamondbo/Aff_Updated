# Service reference
all the members of the services documented here cuz alpaca docs is a mess

# IPostion
**Core Position Data**
* **asset_id:** (UUID) The ID of the asset.
* **symbol:** (string) The symbol of the asset.
* **exchange:** (string) The exchange name of the asset.
* **asset_class:** (string) The asset class name (e.g., "us_equity", "crypto").
* **qty:** (string) The number of shares (can be fractional).
* **side:** (string) "long" or "short".
* **marginable:** (boolean) Indicates if the asset is marginable. 

**Valuation and Price**
* **avg_entry_price:** (string) The average entry price of the position.
* **current_price:** (string) The current asset price per share.
* **lastday_price:** (string) The last day's asset price per share.
* **cost_basis:** (string) Total cost basis in dollars.
* **market_value:** (string) Total dollar amount of the position. 

**Performance and Profit/Loss (P&L)**
* **unrealized_pl:** (string) Unrealized profit/loss in dollars.
* **unrealized_plpc:** (string) Unrealized profit/loss percent.
* **unrealized_intraday_pl:** (string) Unrealized profit/loss in dollars for the day.
* **unrealized_intraday_plpc:** (string) Unrealized profit/loss percent for the day.
* **change_today:** (string) Percent change from last day's price