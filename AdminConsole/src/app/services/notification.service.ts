import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';

interface Notification {
  key: string;
  data?: any;
}

@Injectable()
export class NotificationService {
  private _eventBus: Subject<Notification>;
  private separator = ':';

  constructor() {
    this._eventBus = new Subject<Notification>();
  }

  cast(key: string, data?: any) {
    if (typeof key !== 'string' || !key.length) {
      throw new Error('Bad key. Please provide a string');
    }

    this._eventBus.next({ key, data });
  }


  on<T>(key: string): Observable<T> {
    return this._eventBus.asObservable()
      .pipe(filter( event => this.keyMatch(event.key, key)))
      .pipe(map(event => event.data as T ));
  }


  private keyMatch(key, wildcard) {
    return key === wildcard;
  }
}
