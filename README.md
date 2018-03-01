# YrSlackApp
Simple weather forecast slack notifier using the YR api and Azure Functions


Requires an existing azure func app in Azure set up with these application keys:

| Key | Example value | Comment |
| - | - | - |
| SlackWebhookUrl | https://hooks.slack.com/services/{secret} | [Setup](https://my.slack.com/services/new/incoming-webhook/) |
| WEBSITE_TIME_ZONE | Central Europe Standard Time | Your timezone |
| CRON_EXPRESSION | 0 0 7,15 * * * |
| YR_LOCATION_ID | 1-72837 | [Find ID](http://www.yr.no/api/swagger/ui/index#!/Locations/Locations_Search) |

## Run locally

Fetch config:

* Install azure cli  
  `npm install -g azure-functions-core-tools@core`
* Connect to azure  
  `func azure login`
* Find correct subscription  
  `func azure subscriptions list`
* Switch to correct subscription  
  `func azure subscriptions set 42cbd6ae-5f3f-42d5-ac13-5d7f821e8744`
* From `\YrSlackAzFuncApp` folder, fetch configuration (will be written to `local.settings.json`)  
  `func azure functionapp fetch YrSlackAzFuncApp`
* Decrypt configuration  
  `func settings decrypt`

Change the cron expression to something more test-friendly in `local.settings.json`, like `*/10 * * * * *`.

Run:

* From `\YrSlackAzFuncApp\bin\Debug\netstandard2.0` folder  
`func start`