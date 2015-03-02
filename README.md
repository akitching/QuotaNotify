QuotaNotify
===========

A Windows system tray applet for notifying the current user when they are nearing their quota limit.

Created to try to stop my users filling their home directories, which causes merry Hell with my ZFS based storage.

Configuration
-------------
QuotaNotify can pull its configuration from `config.xml` in the install directory and/or the Windows registry. Settings defined in the User's registry hive override the local machine's registry hive, which overrides settings defined in `config.xml`. If no settings are defined in either the registry of `config.xml`, QuotaNotify will use its default values.

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
#### Per machine registry keys:

| Setting          | Registry Key                                            | Type         | Notes                       |
| ---------------- | ------------------------------------------------------- | ------------ | --------------------------- |
| Initial Interval | ``HKLM\SOFTWARE\AmalgamStudios\QuotaNotify\InitialInterval`` | REG_DWORD    |                             |
| Check Interval   | ``HKLM\SOFTWARE\AmalgamStudios\QuotaNotify\CheckInterval``   | REG_DWORD    |                             |
| Warn Percent     | ``HKLM\SOFTWARE\AmalgamStudios\QuotaNotify\WarnPercent``     | REG_DWORD    |                             |
| Warn Below       | ``HKLM\SOFTWARE\AmalgamStudios\QuotaNotify\WarnBelow``       | REG_DWORD    |                             |
| Warn Message     | ``HKLM\SOFTWARE\AmalgamStudios\QuotaNotify\WarnMessage``     | REG_SZ       |                             |
| Obsess           | ``HKLM\SOFTWARE\AmalgamStudios\QuotaNotify\Obsess``          | REG_DWORD    |                             |
| Drives           | ``HKLM\SOFTWARE\AmalgamStudios\QuotaNotify\Drives``          | REG_MULTI_SZ | One drive letter per string |

#### Per user registry keys:
| Setting          | Registry Key                                            | Type         | Notes                       |
| ---------------- | ------------------------------------------------------- | ------------ | --------------------------- |
| Initial Interval | ``HKCU\SOFTWARE\AmalgamStudios\QuotaNotify\InitialInterval`` | REG_DWORD    |                             |
| Check Interval   | ``HKCU\SOFTWARE\AmalgamStudios\QuotaNotify\CheckInterval``   | REG_DWORD    |                             |
| Warn Percent     | ``HKCU\SOFTWARE\AmalgamStudios\QuotaNotify\WarnPercent``     | REG_DWORD    |                             |
| Warn Below       | ``HKCU\SOFTWARE\AmalgamStudios\QuotaNotify\WarnBelow``       | REG_DWORD    |                             |
| Warn Message     | ``HKCU\SOFTWARE\AmalgamStudios\QuotaNotify\WarnMessage``     | REG_SZ       |                             |
| Obsess           | ``HKCU\SOFTWARE\AmalgamStudios\QuotaNotify\Obsess``          | REG_DWORD    |                             |
| Drives           | ``HKCU\SOFTWARE\AmalgamStudios\QuotaNotify\Drives``          | REG_MULTI_SZ | One drive letter per string |

Change Log
----------
### 1.2.1
* Fix bug which causes application to error when the Drives registry key contains an empty string

### 1.2.0
* Add support for per user settings
* Fix bug which causes application to crash if required registry keys are not present
* Change registry key path:
  * Change `Amalgam` to `AmalgamStudios`
  * Add `QuotaNotify` to path
