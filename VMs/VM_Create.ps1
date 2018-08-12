# Create Resource Group
$ResourceGroupName="532Resources"
az group create -n $ResourceGroupName -l eastus2

# list available VMs
az vm image list --all -f windows -o table

# Create VM
$VmName="532Vm"
$AdminPassword=""
az vm create `
    --resource-group $ResourceGroupName `
    --name $VmName --image win2016datacenter `
    --admin-username 532admin `
    --admin-password $AdminPassword

# Show resources created with VM
az resource list -g $ResourceGroupName -o table

# Show VM size
az vm show -g $ResourceGroupName -n $VmName --query "hardwareProfile.vmSize"

# List IPs
az vm list-ip-addresses -g $ResourceGroupName -n $VmName -o table

# Open port 80
az vm open-port --port 80 -g $ResourceGroupName -n $VmName

# Use Custom script to setup web server
az vm extension set `
    --publisher Microsoft.Compute `
    --version 1.8 `
    --name CustomScriptExtension `
    --vm-name $VmName `
    --resource-group $ResourceGroupName `
    --settings '{"commandToExecute":"powershell.exe Install-WindowsFeature -Name Web-Server"}'

# Deallocate VM
az vm deallocate -g $ResourceGroupName -n $VmName

# Delete resources
az group delete -n $ResourceGroupName --yes