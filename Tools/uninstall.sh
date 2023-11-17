#!/usr/bin/env bash

#Check if script is being run as root
if [ "$(id -u)" != "0" ]; then
   echo "This script must be run as root" 1>&2
   exit 1
fi

if [ ! $? = 0 ]; then
   exit 1
else

   systemctl stop rpicasesystem.service
   systemctl disable /etc/systemd/rpicasesystem.service

   rm /usr/local/bin/rpicasesystem/* -r 
   rm /usr/local/bin/rpicasesystem -d
   rm /etc/systemd/system/rpicasesystem.service

   whiptail --title "Uninstall complete" --msgbox "rpicasesystem uninstall complete.\nYou are safe to remove the folder rpicasesystem/latest" 8 78
fi
