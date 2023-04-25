---
sidebar_position: 5
---

# Set up a Ubuntu Linux Environment to Run WfExS

In the [hutch monorepo](https://github.com/HDRUK/hutch) there is an [Ansible](https://www.ansible.com/) playbook which you can use to quickly build an Ubuntu Linux environment for running WfExS.

Ansible lets you set up a VM from your laptop without installing anything on your VM beforehand.

:::note Before you start
You will need a Ubuntu machine such as a VM to use this playbook. Get one from your favourite cloud provider. Alternatively use VirtualBox, WSL2, etc.
:::

## Optional
Before starting you may wish to create a non-root user in your VM.
`ssh` into your VM as `root` and create a new user with the following command:
```bash
adduser <your new username>
```
Following the instructions and then give the user sudo with:
```bash
usermod -aG sudo <your new username>
```
For Ansible to use sudo as your new user, type `visudo` into the terminal, hit enter, and add the following to the bottom of the file:
```
<your new username>     ALL=(ALL) NOPASSWD:ALL
```
Then save the file.

You should also add a file to the new user's home directory called `.ssh/authorized_keys` and add your public ssh key to it for extra security. Make sure the file is owned by the new user.

## Step 1
Install Ansible on your local machine using `pip`.
```bash
pip install ansible
```

## Step 2
In `ansible/inventory.ini` add the IP address(es) or host URL(s) under the section `[tre_server]` where you wish to set up WfExS.

## Step 3
Run the playbook
```bash
ansible-playbook -i inventory.ini -u <VM username> playbook.yml
```

Depending on the resources your VM has, your WfExS environment will be ready after several minutes.
