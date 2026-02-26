import { ApplicationInsights } from '@microsoft/applicationinsights-web';

const appInsights = new ApplicationInsights({
    config: {
        connectionString: 'InstrumentationKey=c4ac7ac4-8d31-409f-9742-937b587cce35;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/;ApplicationId=73de3d55-6717-4eef-92d9-16b3e01a8a57'
    }
});

appInsights.loadAppInsights();
appInsights.trackPageView(); // Start tracking page views

export { appInsights };
