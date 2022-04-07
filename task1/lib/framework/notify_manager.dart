import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import 'package:timezone/timezone.dart' as tz;
import 'package:flutter_native_timezone/flutter_native_timezone.dart';
import 'package:todolist/framework/app_colors.dart';

class NotifyManager {

    static Future _notificationDetails() async {
        return NotificationDetails(
            android: AndroidNotificationDetails(
                "task_notify_channel",
                "Tasks Notifications",
                icon: "@mipmap/ic_launcher",
                priority: Priority.max,
                importance: Importance.max,
                playSound: true,
                enableVibration: true,
                color: AppColors.primary,
            ),
        );
    }

    static void planNotification({
        required int id,
        required String title,
        required String body,
        required DateTime date,
    }) async {
        try {
            var tzDate =  tz.TZDateTime.from(date, tz.getLocation(await FlutterNativeTimezone.getLocalTimezone()));
            await _notifications.zonedSchedule(
                id,
                title,
                body,
                tzDate.toLocal(),
                await _notificationDetails(),
                uiLocalNotificationDateInterpretation: UILocalNotificationDateInterpretation.absoluteTime,
                androidAllowWhileIdle: true
            );
        } catch (e) { }
    }

    static void removeNotification({
        required int id,
    }) async {
        _notifications.cancel(id);
    }

    static final _notifications = FlutterLocalNotificationsPlugin();
}