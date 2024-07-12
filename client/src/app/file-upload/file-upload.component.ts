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
  uploadResponses: string[] = [];
  uploadFailed: boolean = false;

  constructor(private fileUploadService: FileUploadService) {}

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const files = Array.from(input.files); // Reset the selected files to the new selection
      this.selectedFiles.push(...files); // Adicionar novos arquivos ao array existente
      this.uploadResponses = []; // Clear previous responses when new files are selected
      this.uploadFailed = false; // Reset upload failure status
    }
    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }
  }

  cancelSelection() {
    this.selectedFiles = [];
    this.uploadResponses = [];
    this.uploadFailed = false; // Reset upload failure status
    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }
  }

  upload() {
    if (this.selectedFiles.length === 0) {
      this.uploadResponses.push('Please select at least one file first!');
      return;
    }

    this.selectedFiles.forEach(file => {
      this.fileUploadService.uploadFile(file).subscribe({
        next: (response: {message: string}) => {
          this.uploadResponses.push(`File uploaded successfully!`);
          this.uploadFailed = false;
        },
        error: (err) => {
          console.error("FAILED!!!!!", err);
          this.uploadResponses.push(`Upload failed`);
          this.uploadFailed = true;
        }
      })
    });
  }

  tryAgain() {
    this.upload(); // Re-attempt the upload
  }

  removeLastFile() {
    if (this.selectedFiles.length > 0) {
      this.selectedFiles.pop();
    }
  }
}
