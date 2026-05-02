import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import PrivateRoute from './components/PrivateRoute';
import Header from './components/Header';

import Login from './pages/Login';
import ArticlesList from './pages/ArticlesList';
import ArticleDetail from './pages/ArticleDetail';
import ArticleForm from './pages/ArticleForm';
import ArticleEdit from './pages/ArticleEdit';
import UploadPdf from './pages/UploadPdf';

import './App.css';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <div className="app">
          <Header />
          <main className="app-main">
            <Routes>
              <Route path="/login" element={<Login />} />

              <Route
                path="/"
                element={
                  <PrivateRoute>
                    <ArticlesList />
                  </PrivateRoute>
                }
              />

              <Route
                path="/article/:id"
                element={
                  <PrivateRoute>
                    <ArticleDetail />
                  </PrivateRoute>
                }
              />

              <Route
                path="/create"
                element={
                  <PrivateRoute>
                    <ArticleForm />
                  </PrivateRoute>
                }
              />

              <Route
                path="/edit/:id"
                element={
                  <PrivateRoute>
                    <ArticleEdit />
                  </PrivateRoute>
                }
              />

              <Route
                path="/upload"
                element={
                  <PrivateRoute>
                    <UploadPdf />
                  </PrivateRoute>
                }
              />
            </Routes>
          </main>
        </div>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
