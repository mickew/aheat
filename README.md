# Askholmen heat system
## Askholmen heat system

### Databese migrations

Add-Migration InitialCreate -OutputDir Data\Migrations

Update-Database

### Install
sudo apt-get update

sudo apt-get upgrade

sudo apt-get install nginx

sudo ./install.sh

sudo nginx -s reload