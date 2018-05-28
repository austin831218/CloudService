// NOTE: these are depdencies order sensitive!!!
export * from './notification.service';
export * from './jsonClient.service';


export const EventContract = {
  ScreenLock: 'lock',
  BoEvents: {
    Updated: 'bo:updated',
    Created: 'bo:created',
    Deleted: 'bo:deleted'
  }
};


