Deploy function to AWS Lambda
```
    dotnet lambda deploy-function --function-name nigel-lambdasimple

    set DATADOG_API_KEY=<api-key>
    set DATADOG_SITE=datadoghq.com
    set DATADOG_SERVICE=nigel-lambdasimple
    set DATADOG_VERSION=V1.0
    set DATADOG_ENV=dev

    datadog-ci lambda instrument -r us-east-1 -f nigel-lambdasimple  -i 


     dotnet lambda invoke-function --function-name nigel-lambdasimple --payload "foo"
```

