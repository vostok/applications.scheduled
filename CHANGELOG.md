## 0.2.8 (12-05-2021):

Implemented dynamic configuration of scheduled actions set (https://github.com/vostok/applications.scheduled/issues/1).

## 0.2.7 (25-03-2021):

Added possibility to use `Demand` with arbitrary argument.

## 0.2.6 (19-02-2021):

Added `VostokScheduledAsyncApplication` to support async initialization methods.

## 0.2.5 (04-02-2021):

Trace with custom operations spans.

## 0.2.4 (03-12-2020):

- Added `DoDisposeAsync` virtual method.

## 0.2.3 (20-07-2020):

Fixed https://github.com/vostok/applications.scheduled/issues/6

## 0.2.2 (15-07-2020):

- ScheduledActionRunner: do no log time budget violations when using PeriodicalWithConstantPauseScheduler.
- PeriodicalWithConstantPauseScheduler: added delayFirstIteration parameter.

## 0.2.1 (14-07-2020):

Added new scheduler: PeriodicalWithConstantPause.

## 0.2.0 (27-06-2020):

- Implemented diagnostic info providers
- Implemented error-based health check
- Added `AddScheduled` extension for `ICompositeApplicationBuilder`
- Enabled custom Dispose

## 0.1.2 (26-05-2020):

Added `Timestamp` field to `IScheduledActionContext`.

## 0.1.1 (20-05-2020):

- Implemented https://github.com/vostok/applications.scheduled/issues/2
- Implemented https://github.com/vostok/applications.scheduled/issues/3

## 0.1.0 (06-02-2020):

Initial release.