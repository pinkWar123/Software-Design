import axios from 'axios';

// Tạo instance axios với cấu hình mặc định
const axiosInstance = axios.create({
  baseURL: 'http://localhost:5215/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor
axiosInstance.interceptors.request.use(
  (config) => {
    // Lấy token từ localStorage
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor
axiosInstance.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response) {
      // Xử lý các lỗi response (401, 403, 500, etc.)
      switch (error.response.status) {
        case 401:
          // Xử lý lỗi unauthorized
          localStorage.removeItem('token');
          // Có thể chuyển hướng đến trang đăng nhập ở đây
          break;
        case 403:
          // Xử lý lỗi forbidden
          break;
        default:
          // Xử lý các lỗi khác
          break;
      }
    }
    return Promise.reject(error);
  }
);

export default axiosInstance;
