import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api/v1';

const api = axios.create({
  baseURL: API_BASE_URL,
});

// Add JWT token to every request
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Handle responses and redirect on 401
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const authService = {
  login: (email, password) =>
    api.post('/auth/login', { email, password }),
};

export const articleService = {
  getAll: (page = 1, pageSize = 10) =>
    api.get('/articles', { params: { page, pageSize } }),

  search: (query, page = 1, pageSize = 10) =>
    api.get('/articles/search', { params: { query, page, pageSize } }),

  getById: (id) =>
    api.get(`/articles/${id}`),

  create: (data) =>
    api.post('/articles', data),

  update: (id, data) =>
    api.put(`/articles/${id}`, data),

  delete: (id) =>
    api.delete(`/articles/${id}`),

  upload: (formData) =>
    api.post('/articles/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    }),

  resummarize: (id) =>
    api.post(`/articles/${id}/resummarize`),
};

export default api;
