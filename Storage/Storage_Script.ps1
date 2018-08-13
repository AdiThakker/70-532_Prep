# Create Resource Group
$ResourceGroupName="532Resources"
$Location="eastus2"
$StorageAccount="532s"

az group create -n $ResourceGroupName -l $Location

# List Resource Group
az group list -o table

# Create Standard Storage Account
az storage account create -n $StorageAccount -g $ResourceGroupName -l $Location --sku Standard_LRS

# Store the Connection String
$ConnectionString= az storage account show-connection-string -n $StorageAccount -g $ResourceGroupName --query connectionString -o tsv

# create blob containers
az storage container create -n "public" --public-access blob --connection-string $ConnectionString

az storage container create -n "private" --public-access off --connection-string $ConnectionString

# Upload files
az storage blob upload -c "public" -f "C:\files\Sample.txt" -n "Sample.txt" --connection-string $ConnectionString

# get the file url for public blob
az storage blob url -c "public" -n "Sample.txt" -o tsv --connection-string $ConnectionString

# generate SAS
# az storage blob generate-sas -c "private" --permissions r -o tsv --expiry 

# ========== Queue ============
# Create Queue
$QueueName = "myqueue"
az storage queue create -n $QueueName --connection-string $ConnectionString

# send message
az storage message put --content "Hello World!" -q $QueueName --connection-string $ConnectionString

# get and delete message
az storage message get -q $QueueName --visibility-timeout 120 --connection-string $ConnectionString
az storage message delete --id "ec947d7c-7d29-4c46-bd00-072921fc33e2" --pop-receipt "AgAAAAMAAAAAAAAAHOnTQZgy1AE=" -q $QueueName --connection-string $ConnectionString


# ========== Table =============
# Create table
$TableName="myTable"
az storage table create -n $TableName --connection-string $ConnectionString