import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Router } from '@angular/router';
import { NotificationService } from './notification.service';
import 'rxjs/add/operator/toPromise';
import { LocalStorageService } from 'ngx-webstorage';
import { ToastrService } from 'ngx-toastr';
import { AppConfig as cfg } from '../../environments/environment';

@Injectable()
export class JsonClientService {

  constructor(
    private http: Http,
    private ls: LocalStorageService,
    private toastrManager: ToastrService,
    private ns: NotificationService,
    private router: Router
  ) { }

  get(uri: string,
    toastrError: boolean = true,
    autoUnlock: boolean = true,
    options?: RequestOptions): Promise<any> {

    this.ns.cast('lock', true);
    return this.http.get(`${cfg.apiEndpoint}${uri}`, this.setRequestOptions(options)).toPromise().then(data => {
      console.debug(`getting from ${uri}`, data);
      if (autoUnlock) {
        this.ns.cast('lock', false);
      }
      return Promise.resolve(data.text() ? data.json() : '');
    }).catch(err => {
      this.errorHandler(err, toastrError, true);
      return Promise.reject(err);
    });
  }

  post(uri: string,
    body: any,
    toastrError: boolean = true,
    autoUnlock: boolean = true,
    options?: RequestOptions): Promise<any> {

    this.ns.cast('lock', true);
    return this.http.post(`${cfg.apiEndpoint}${uri}`, body, this.setRequestOptions(options)).toPromise().then(data => {
      console.debug(`posting to ${uri}`, data);
      if (autoUnlock) {
        this.ns.cast('lock', false);
      }
      return Promise.resolve(data.text() ? data.json() : '');
    }).catch(err => {
      this.errorHandler(err, toastrError, true);
      return Promise.reject(err);
    });
  }

  put(uri: string,
    body: any,
    toastrError: boolean = true,
    autoUnlock: boolean = true,
    options?: RequestOptions): Promise<any> {

    this.ns.cast('lock', true);
    return this.http.put(`${cfg.apiEndpoint}${uri}`, body, this.setRequestOptions(options)).toPromise().then(data => {
      console.debug(`putting to ${uri}`, data);
      if (autoUnlock) {
        this.ns.cast('lock', false);
      }
      return Promise.resolve(data.text() ? data.json() : '');
    }).catch(err => {
      this.errorHandler(err, toastrError, true);
      return Promise.reject(err);
    });
  }

  delete(uri: string,
    toastrError: boolean = true,
    autoUnlock: boolean = true,
    options?: RequestOptions): Promise<any> {

    this.ns.cast('lock', true);
    return this.http.delete(`${cfg.apiEndpoint}${uri}`, this.setRequestOptions(options)).toPromise().then(data => {
      console.debug(`deleting from ${uri}`, data);
      if (autoUnlock) {
        this.ns.cast('lock', false);
      }
      return Promise.resolve(data.text() ? data.json() : '');
    }).catch(err => {
      this.errorHandler(err, toastrError, true);
      return Promise.reject(err);
    });
  }



  private setRequestOptions(options?: RequestOptions): RequestOptions {
    const opt = options || new RequestOptions({ headers: new Headers() });
    // opt.headers.set('Cache-Control', 'no-cache');
    // opt.headers.set('Pragma', 'no-cache');
    const tk = this.ls.retrieve('token');
    if (tk && tk.token) {
      opt.headers.set('Authorization', tk.token);
    }
    return opt;
  }

  private errorHandler(ex, toast: boolean, unlock: boolean) {
    console.error(ex);
    if (toast) {
      this.toastrError(ex);
    }
    if (unlock) {
      this.ns.cast('lock', false);
    }
    if (ex.status === 401) {
      this.ns.cast('lock', false);
      this.router.navigate(['/login']);
    }
  }

  private toastrError(ex) {
    let err: any;
    try {
      err = ex.json();
      console.log('error', err);
    } catch (exception) {
      console.error(exception);
      err = { error: 'unknow', message: 'unknow error' };
    }
    this.toastrManager.error(err.message || ex, err.error || ex.status);
  }

}
