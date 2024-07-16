import { Component, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { FileUploadService } from '../_services/file-upload.service';

@Component({
  selector: 'app-file-upload',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css']
})
export class FileUploadComponent {
  @ViewChild('fileInput', { static: false }) fileInput!: ElementRef;
  selectedFiles: File[] = [];
  uploadResponses: { scanResult: string }[] = [];
  uploadFailed: boolean = false;
  loading: boolean = false;

  constructor(private fileUploadService: FileUploadService) { }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const files = Array.from(input.files);
      this.selectedFiles.push(...files);
      this.uploadResponses = [];
      this.uploadFailed = false;
      this.loading = false;
    }
    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }
  }

  cancelSelection() {
    this.selectedFiles = [];
    this.uploadResponses = [];
    this.uploadFailed = false;
    this.loading = false;
    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }
  }

  upload() {
    if (this.selectedFiles.length === 0) {
      this.uploadResponses.push({ scanResult: 'Please select at least one file first!' });
      return;
    }

    this.loading = true;
    const uploadObservables = this.selectedFiles.map(file =>
      this.fileUploadService.uploadFile(file).toPromise()
        .then(response => {
          const scanResult = JSON.parse(response.scanResult).scanResult; // Extract scanResult
          const resultString = `${file.name}: ${scanResult}`;
          console.log(`Scan result for ${file.name}: ${scanResult}`); // Print scan result to terminal
          return { fileName: file.name, scanResult: resultString };
        })
        .catch(() => {
          const resultString = `${file.name}: Failed`;
          console.log(`Scan result for ${file.name}: Failed`); // Print failed result to terminal
          return { fileName: file.name, scanResult: resultString };
        })
    );

    Promise.all(uploadObservables).then(results => {
      this.uploadResponses = results;
      this.uploadFailed = results.some(result => result.scanResult === 'Failed');
      this.loading = false;
    }).catch(error => {
      this.uploadFailed = true;
      this.loading = false;
    });
  }

  tryAgain() {
    this.upload();
  }

  removeLastFile() {
    if (this.selectedFiles.length > 0) {
      this.selectedFiles.pop();
    }
  }
}
