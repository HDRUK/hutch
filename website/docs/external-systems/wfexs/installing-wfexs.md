---
sidebar_position: 1
---

# Installing WfExS

## System requirements
It is recommended to use WfExS on Ubuntu. Use WSL on Windows or [UTM](https://mac.getutm.app/) on MacOS.

:::tip Ubuntu on Apple Silicon (ARM64)
Create a VM in UTM using an `arm64` version of Ubuntu. First download the [server edition](https://ubuntu.com/download/server/arm). Then once installed, login and run:
```shell
sudo apt install ubuntu-desktop
```
Then restart the VM.
:::

## WfeXs Installation Requirements
WfeXs requires a number of other applications to work. Full information for installing WfExS can be found at [https://github.com/inab/WfExS-backend/blob/main/INSTALL.md](https://github.com/inab/WfExS-backend/blob/main/INSTALL.md).

### Installation Steps (tested on a linux VM)
Make sure you have previously installed curl, tar, gzip, python3 and its pip and venv counterparts. 
WfeXs also requires the following [software dependencies](https://github.com/inab/WfExS-backend/blob/main/INSTALL.md#software-dependencies).

1. Create python virtual environment
    ```shell
    python3 -m venv .pyWEenv
    source .pyWEenv/bin/activate
    ```
2. Run automated installer.
    ```shell
    ./full-installer.bash
    ```
3. Modify line 143 in singularity-local-installer.bash to include --without-seccomp --without-conmon flags.
    ```shell
    ./mconfig -b ./builddir --without-suid --without-seccomp --without-conmon --prefix="${envDir}"
    ```
4. Run singularity installer
    ```shell
    ./singularity-local-installer.bash
    ```
