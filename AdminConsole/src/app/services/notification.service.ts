import { Injectable } from '@angular/core';
import { Subject, Observable, of, interval } from 'rxjs';
import { filter, map, delay } from 'rxjs/operators';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
import { AppConfig as cfg } from '../../environments/environment';
import { Message } from '../Models/Message';



interface Notification {
  key: string;
  data?: any;
}

@Injectable()
export class NotificationService {
  private _eventBus: Subject<Notification>;
  private _wsBus: Subject<Message>;
  private socket$: WebSocketSubject<any>;
  private separator = ':';

  constructor() {
    this._eventBus = new Subject<Notification>();
    this._wsBus = new Subject<Message>();
    this.socket$ = webSocket(`${cfg.apiEndpoint}`);
    this.connectWs();
    interval(5000)
      .subscribe(x => {
        this.sendWSCommand({ Type: 'Ping' });
      })
  }

  connectWs() {
    this.socket$
      .subscribe(
        (message) => {
          this._wsBus.next(message);
        },
        (err) => {
          console.error(err);
          console.warn('re-connect ws in 5 seconds');
          of(null).pipe(delay(5000)).subscribe(() => {
            this.connectWs();
          });
        },
        () => console.warn('Completed!')
      );
  }

  cast(key: string, data?: any) {
    if (typeof key !== 'string' || !key.length) {
      throw new Error('Bad key. Please provide a string');
    }

    this._eventBus.next({ key, data });
  }


  on<T>(key: string): Observable<T> {
    return this._eventBus.asObservable()
      .pipe(filter(event => this.keyMatch(event.key, key)))
      .pipe(map(event => event.data as T));
  }

  onWsMessage(f: (m: Message) => boolean): Observable<Message> {
    return this._wsBus.asObservable()
      .pipe(filter(event => f(event)));
  }

  sendWSCommand(cmd) {
    this.socket$.next(cmd);
  }


  private keyMatch(key, wildcard) {
    return key === wildcard;
  }
}
