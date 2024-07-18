// ****************************************************************************************************************
//                 This Angular service handles file uploads to a specified API endpoint. 
//  It uses HttpClient to send a POST request with the file data and returns the server response as an Observable.
// ****************************************************************************************************************

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
  baseUrl = 'http://localhost:5011/api/';

  constructor(private http: HttpClient) { }

  uploadFile(file: File): Observable<any> {
    const formData: FormData = new FormData();
    formData.append('file', file, file.name);

    return this.http.post<{ fileName: string, scanResult: string }>(this.baseUrl + 'upload', formData, { withCredentials: true });
  }
}
