import 'package:flutter/material.dart';
import 'package:todolist/framework/app_text.dart';
import 'package:todolist/screens/home_screen_widget.dart';

import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:timezone/data/latest.dart' as tz;

void main() {
    tz.initializeTimeZones();

    var homeScreen = HomeScreenWidget();

    runApp(
        MaterialApp(
            title: AppText.titleApp,
            home: homeScreen,
            routes: {
                HomeScreenWidget.id: (context) => homeScreen,
            },
            localizationsDelegates: [
                GlobalMaterialLocalizations.delegate,
                GlobalCupertinoLocalizations.delegate,
                GlobalWidgetsLocalizations.delegate,
            ],
            supportedLocales: [
                const Locale('ru'),
            ],
        ),
    );
}
