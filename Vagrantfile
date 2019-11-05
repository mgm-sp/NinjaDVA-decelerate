# -*- mode: ruby -*-
# vi: set ft=ruby :

# ninjaDVA decelerate
#
# Requirements:
# Please execute `vagrant plugin install vagrant-triggers`
# if you start the vm the first time and get an error

Vagrant.configure("2") do |config|

  #-------------------Your VM config----------------------------------
  config.vm.define "decelerate"

  # name and version of vm image
  config.vm.box = "bento/debian-10"

  # set hostname for vm
  config.vm.hostname = "decelerate"

  # disable standard synced folder
  config.vm.synced_folder ".", "/vagrant", disabled: true

  # install docker
  config.vm.provision "shell", inline: <<-END
    apt-get -y update
    apt-get -y install apt-transport-https ca-certificates curl gnupg2 software-properties-common
    curl -fsSL https://download.docker.com/linux/debian/gpg | apt-key add -
    add-apt-repository -y "deb [arch=amd64] https://download.docker.com/linux/debian $(lsb_release -cs) stable"
    apt-get -y update
    apt-get -y install docker-ce
  END

  # copy sources to the guest system
  config.vm.provision "file", source: "decelerate", destination: "decelerate"

  # build docker container
  config.vm.provision "shell", inline: <<-END
    docker build -t decelerate decelerate
  END

  # stop docker container if it runs already, then start it
  config.vm.provision "shell", run: "always", inline: <<-END
    docker stop decelerate
    docker run --rm -d -p 80:80 --name decelerate decelerate
  END


  #----------------- ninjaDVA specific configuration -------------------------------
  # test whether the vm is started in ninjaDVA context
  # if yes copy challenges to dashboard vm
  if File.exists?("../ninjadva.rb")
    require "../ninjadva"
    NinjaDVA.new(config)
  end

end
