host=$1
servicename=$2
path=$3

clear
echo "-= SERVICE STOP  =-"
ssh root@$host ''systemctl is-active $servicename''
ssh root@$host ''systemctl stop $servicename''
ssh root@$host ''systemctl is-active $servicename''

echo "-= COPY FILES =-"
rsync -r --info=progress2 --exclude appsettings*.json _Published/ root@$host:$path
rsync -r --ignore-existing --info=progress2 _Published/appsettings*.json root@$1:$path

echo "-= SERVICE START =-"
ssh root@$host ''systemctl start $servicename''

echo "-= SERVICE STATUS =-"
ssh root@$host ''systemctl status $servicename''
