import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';


@Injectable({ providedIn: 'root' })
export class ApiService {
  public readonly API_URL = environment.apiUrl;
  public readonly HUB_URL = environment.hubUrl;
}
