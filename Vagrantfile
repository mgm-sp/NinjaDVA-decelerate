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

  # remove old sources from the guest system
  config.vm.provision "shell", inline: <<-END
    rm -rf decelerate
  END

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

  # install PhantomJS
  config.vm.provision "shell", inline: <<-END
    apt-get -y install libfontconfig1
    cd /tmp
    wget --quiet https://bitbucket.org/ariya/phantomjs/downloads/phantomjs-2.1.1-linux-x86_64.tar.bz2
    tar xf phantomjs-2.1.1-linux-x86_64.tar.bz2
    cp phantomjs-2.1.1-linux-x86_64/bin/phantomjs /usr/local/bin
    rm -rf phantomjs-2.1.1-linux-x86_64{,.tar.bz2}
  END

  # install CasperJS
  config.vm.provision "shell", inline: <<-END
    apt-get -y install git
    rm -rf casperjs
    git clone git://github.com/casperjs/casperjs.git
    cd casperjs
    ln -sf `pwd`/bin/casperjs /usr/local/bin/casperjs
  END

  # setup API webserver
  config.vm.provision "file", source: "attacks/websocket/apiserver.service", destination: "apiserver.service"
  config.vm.provision "file", source: "attacks/websocket/presenter.cgi", destination: "api/cgi-bin/presenter.cgi"
  config.vm.provision "file", source: "attacks/websocket/presenter.js", destination: "presenter.js"
  config.vm.provision "shell", inline: <<-END
    mv apiserver.service /etc/systemd/system/
    chmod +x api/cgi-bin/presenter.cgi
    systemctl daemon-reload
    systemctl enable apiserver.service
    systemctl restart apiserver.service
  END



  #----------------- ninjaDVA specific configuration -------------------------------
  # test whether the vm is started in ninjaDVA context
  # if yes copy challenges to dashboard vm
  if File.exists?("../ninjadva.rb")
    require "../ninjadva"
    NinjaDVA.new(
      config,
      {
        link_widget_links: [
          { :hostname => "decelerate", :name => "decelerate" },
        ]
      }
    )
  end

end
