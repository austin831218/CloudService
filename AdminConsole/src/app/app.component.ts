import { Component } from '@angular/core';
import { ElectronService } from './providers/electron.service';
import { TranslateService } from '@ngx-translate/core';
import { AppConfig } from '../environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  private loglevels = ['all', 'debug', 'info', 'warn', 'error'];

  constructor(public electronService: ElectronService,
    private translate: TranslateService) {

    translate.setDefaultLang('en');
    console.log('AppConfig', AppConfig);

    if (electronService.isElectron()) {
      console.log('Mode electron');
      console.log('Electron ipcRenderer', electronService.ipcRenderer);
      console.log('NodeJS childProcess', electronService.childProcess);
    } else {
      console.log('Mode web');
    }

    if (AppConfig.logLevel > 0) {
      window['console']['_log'] = window['console']['log'];
      window['console']['log'] = function () { };
    }

    for (let i = 1; i < AppConfig.logLevel; i++) {
      window['console']['_' + this.loglevels[i]] = window['console'][this.loglevels[i]];
      window['console'][this.loglevels[i]] = function () { };
    }

    console.debug('debug');
    console.info('info');
    console.warn('warn');
    console.error('error');
  }
}
