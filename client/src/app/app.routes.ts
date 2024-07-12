import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { FileUploadComponent } from './file-upload/file-upload.component';

export const appRoutes: Routes = [
  { path: '', component: HomeComponent },  // Rota padrão para HomeComponent
  { path: 'upload', component: FileUploadComponent },
  { path: '**', redirectTo: '', pathMatch: 'full' }  // Redireciona rotas não encontradas para a Home
];
