

# Create Web App - ARM
New-AzureRmWebApp -ResourceGroupName "532Resources" -Name "532WebApps" `
    -Location "eastus2" -AppServicePlan "532AppPlan"
# CLI
 az webapp create --name 532WebApps --plan 532AppPlan --resource-group 532Resources

# Get Web App settings
$WebApp = Get-AzureRmWebAppSlot -ResourceGroupName "532Resources" -Name "532WebApps" -Slot production

$WebApp.SiteConfig.AppSettings

# CLI
az webapp config appsettings set --resource-group 532Resources --name 532WebApps  --settings "AppSetting1=On"

# Get Connection Strings
az webapp config connection-string list --resource-group 532Resources --name 532WebApps

# Delete Resources
az group delete --name 532Resources --yes