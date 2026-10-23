import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ClientRequest {
  id: number;
  clientId: number;
  clientName: string;
  advisorId: number;
  requestType: string;
  subject: string;
  message: string;
  status: string;
  advisorResponse: string;
  submittedAt: string;
  resolvedAt: string | null;
}

export interface CreateRequest {
  requestType: string;
  subject: string;
  message: string;
}

export interface RespondToRequest {
  advisorResponse: string;
  status: string;
}

@Injectable({ providedIn: 'root' })
export class RequestService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/clientrequests`;

  getMyRequests(): Observable<ClientRequest[]> {
    return this.http.get<ClientRequest[]>(`${this.base}/my`);
  }

  getAdvisorInbox(): Observable<ClientRequest[]> {
    return this.http.get<ClientRequest[]>(`${this.base}/inbox`);
  }

  submitRequest(dto: CreateRequest): Observable<ClientRequest> {
    return this.http.post<ClientRequest>(this.base, dto);
  }

  respondToRequest(id: number, dto: RespondToRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/${id}/respond`, dto);
  }
}
