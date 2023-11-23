# Askholmen heat system
[![Build](https://github.com/mickew/aheat/actions/workflows/build.yml/badge.svg)](https://github.com/mickew/aheat/actions/workflows/build.yml)
[![Deploy](https://github.com/mickew/aheat/actions/workflows/deploy.yml/badge.svg)](https://github.com/mickew/aheat/actions/workflows/deploy.yml)
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
