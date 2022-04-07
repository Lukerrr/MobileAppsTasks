class UserTask {

    UserTask({required String name, DateTime? notificationTime})
        :   this.name = name,
            this.notificationTime = notificationTime,
            bIsDone = false;

    UserTask.fromJson(Map<String, dynamic> json) :
        name = json['name'],
        notificationTime = json['notify'] != null
            ? DateTime.tryParse(json['notify'])
            : null,
        bIsDone = json['done'];

    Map<String, dynamic> toJson() => {
        'name': name,
        'notify': notificationTime?.toIso8601String(),
        'done': bIsDone,
    };

    String name;
    DateTime? notificationTime;
    bool bIsDone;
}
