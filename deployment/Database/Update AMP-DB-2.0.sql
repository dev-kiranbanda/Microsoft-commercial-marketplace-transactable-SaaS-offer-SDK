
Insert into ApplicationConfiguration
select 'EmailLogo','https://securithings.com/images/logo_2020_highres-2.png','Email Logo' Union all
select 'ApplicationLogo','https://securithings.com/images/logo_2020_highres-2.png','Application Logo' Union all
select 'Footer',' <div class="container text-center">  &copy; 2020 SecuriThings &reg; All Rights Reserved. </div>','Footer' UNION ALL
select 'HomePageContent',' <div class="pt-5">
        <div class="card p-3 ">
            <div class="text-center">
                <h1 class="display-4">Welcome</h1>
            </div>
            <div class="text-left">
                <p> Get started with integrating your <b> Software-as-a-service </b> application with the <b>SaaS Fulfilment APIs (v2.0)</b>  .</p>
                <p><a href="https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2" target="_blank">Click here </a><span>for details on the Fulfilment API.</span> </p>
            </div>
            <div>
                <b>To purchase this SAAS offering:</b>
                <ul>
                    <li>Log on to <a href="https://portal.azure.com" target="_blank">Azure</a> </li>
                    <li>Search for resources of type <b> Software-as-a-service </b></li>
                    <li>Search for <b>Cloud SaaS Kit</b></li>
                    <li>Click <b> Create</b> to initiate the deployment of the SaaS resource</li>
                    <li>Click <b>Configure Account </b> after the resource is created</li>
                </ul>
            </div>
            <div>
                <p>If you already had activated subscriptions, click Subscriptions to see a list of your existing subscriptions.</p>
                <p>You could switch plans on existing subscriptions or cancel one of your subscriptions using this portal.</p>
            </div>
        </div>
    </div>
','Home Page Content' 
