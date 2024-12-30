export class TimeZoneOffset {
  static getTimezoneOffsetInHours() {
    return -new Date().getTimezoneOffset() / 60;
  }
}
