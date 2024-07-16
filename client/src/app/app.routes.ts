import { Routes } from '@angular/router';
import { FileUploadComponent } from './file-upload/file-upload.component';

export const routes: Routes = [
  { path: '', redirectTo: '/upload', pathMatch: 'full' }, // Redirecionar para o componente de upload
  { path: 'upload', component: FileUploadComponent },
];
