QuotaNotify
===========

A Windows system tray applet for notifying the current user when they are nearing their quota limit.

Created to try to stop my users filling their home directories, which causes merry Hell with my ZFS based storage.

Configuration
-------------
QuotaNotify can be pull its configuration from `config.xml` in the install directory and/or the Windows registry. Settings defined in the registry will override settings defined in `config.xml`. If no settings are defined in either the registry of `config.xml`, QuotaNotify will use its default values.

### Settings
#### Initial Interval
How long to wait (in milliseconds) after application start before first check.  
**Default:** 5000 (5 seconds)

#### Check Interval
How long to wait (in milliseconds) between disk checks.  
**Default:** 300000 (5 minutes)

#### Warn Percent
Warn when available space on a monitored drive is below this value (percentage of drive).  
**Default:** 10

#### Warn Below
Do not warn if available space is greater than this (in bytes). Stops low space warnings for large drives/quotas where a low percentage can still provide adequate storage (i.e. 10% of 10G).  
**Default:** 104857600 (100MB)

#### Warn Message
A custom string to add to the warning dialog.  
**Default:** _none_

#### Obsess
Notify user of low space even if free space is not smaller than during the previous check; to annoy the Hell out of users who ignore the first warning >:)  
**Default:** false

#### Drives
List of drive letters to monitor.  
**Default:** H

### XML
#### Example
```xml
<?xml version='1.0'?>
<Config>
    <initialInterval>5000</initialInterval>
    <checkInterval>300000</checkInterval>
    <warnPercent>10</warnPercent>
    <warnBelow>104857600</warnBelow>
    <warnMessage>It is better to clear space now than to have a save operation fail because you have no space left.</warnMessage>
    <obsess>false</obsess>
    <Drives>
        <Drive letter="E" />
        <Drive letter="Q" />
        <Drive letter="U" />
    </Drives>
</Config>
```

### Registry
#### Registry keys for QuotaNotify:

| Setting          | Registry Key                                            | Type         | Notes                       |
| ---------------- | ------------------------------------------------------- | ------------ | --------------------------- |
| Initial Interval | ``HKLM\SOFTWARE\Amalgam\InitialInterval`` | REG_DWORD    |                             |
| Check Interval   | ``HKLM\SOFTWARE\Amalgam\CheckInterval``   | REG_DWORD    |                             |
| Warn Percent     | ``HKLM\SOFTWARE\Amalgam\WarnPercent``     | REG_DWORD    |                             |
| Warn Below       | ``HKLM\SOFTWARE\Amalgam\WarnBelow``       | REG_DWORD    |                             |
| Warn Message     | ``HKLM\SOFTWARE\Amalgam\WarnMessage``     | REG_SZ       |                             |
| Obsess           | ``HKLM\SOFTWARE\Amalgam\Obsess``          | REG_DWORD    |                             |
| Drives           | ``HKLM\SOFTWARE\Amalgam\Drives``          | REG_MULTI_SZ | One drive letter per string |
