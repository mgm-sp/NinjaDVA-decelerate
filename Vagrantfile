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

  # copy sources to the guest system
  config.vm.provision "file", source: "decelerate", destination: "decelerate"

  # install, build, and run docker
  config.vm.provision "docker" do |d|
    d.build_image "decelerate",
      args: "-t decelerate"
    d.run "decelerate",
      image: "decelerate:latest",
      args: "-p 80:80"
  end


  #----------------- ninjaDVA specific configuration -------------------------------
  # test whether the vm is started in ninjaDVA context
  # if yes copy challenges to dashboard vm
  if File.exists?("../ninjadva.rb")
    require "../ninjadva"
    NinjaDVA.new(config)
  end

end