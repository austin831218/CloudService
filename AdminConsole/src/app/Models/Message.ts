export class Message {
  Type: string;
  Level: LogLevel;
  JobName: string;
  JobThreadId: string;
  Ticks: number;
  Data: any;
  Content: string;
}

export class LogLevel {
  Name: string;
  Ordinal: number;
}
