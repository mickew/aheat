#!/usr/bin/env bash

#Check if script is being run as root
if [ "$(id -u)" != "0" ]; then
   echo "This script must be run as root" 1>&2
   exit 1
fi

if [ ! $? = 0 ]; then
   exit 1
else

   chmod +x /var/www/powercontrol/AHeat.Web.API

   # create database folder
   mkdir -p /usr/share/powercontrol
   sudo chmod 777 /usr/share/powercontrol

   /var/www/powercontrol/AHeat.Web.API --seed

   cp powercontrolsystem.service /etc/systemd/system
   if [ ! -f /etc/systemd/system/powercontrolsystem.service ]; then
     whiptail --title "Installation aborted" --msgbox "There was a problem writing the powercontrolsystem.service file" 8 78
    exit
   fi

   if [ ! -f /etc/nginx/sites-available/default.bak ]; then
     cp /etc/nginx/sites-available/default /etc/nginx/sites-available/default.bak
   fi

   cp default /etc/nginx/sites-available/default
   if [ ! -f /etc/nginx/sites-available/default ]; then
     whiptail --title "Installation aborted" --msgbox "There was a problem writing the default nginx config file" 8 78
    exit
   fi
   nginx -s reload

   systemctl enable /etc/systemd/system/powercontrolsystem.service
   systemctl start /etc/systemd/system/powercontrolsystem.service
   whiptail --title "Installation complete" --msgbox "Power Control System installation complete. The system will reboot." 8 78

   #reboot
   #poweroff
fi
