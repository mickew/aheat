# Askholmen heat system
[![Build](https://github.com/mickew/aheat/actions/workflows/build.yml/badge.svg)](https://github.com/mickew/aheat/actions/workflows/build.yml)
[![Deploy](https://github.com/mickew/aheat/actions/workflows/deploy.yml/badge.svg)](https://github.com/mickew/aheat/actions/workflows/deploy.yml)
## Askholmen heat system

### Databese migrations

Add-Migration InitialCreate -OutputDir Data\Migrations

Update-Database

### Install
sudo apt update -y && sudo apt upgrade -y

sudo apt-get install ufw

sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

sudo ufw enable

sudo apt-get install nginx

sudo ./getlatest.sh
sudo ./install.sh

sudo nginx -s reload

sudo apt install snapd

sudo reboot

sudo snap install core; sudo snap refresh core

sudo snap install --classic certbot

sudo ln -s /snap/bin/certbot /usr/bin/certbot

sudo certbot --nginx

