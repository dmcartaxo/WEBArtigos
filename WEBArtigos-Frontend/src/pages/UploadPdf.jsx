import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { articleService } from '../services/api';
import Alert from '../components/Alert';
import './UploadPdf.css';

const UploadPdf = () => {
  const [formData, setFormData] = useState({
    title: '',
    author: '',
    file: null,
  });
  const [loading, setLoading] = useState(false);
  const [alert, setAlert] = useState(null);
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value, files } = e.target;
    if (name === 'file') {
      setFormData((prev) => ({ ...prev, file: files[0] }));
    } else {
      setFormData((prev) => ({ ...prev, [name]: value }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.title.trim() || !formData.author.trim() || !formData.file) {
      setAlert({
        type: 'error',
        message: 'Por favor, preencha todos os campos e selecione um PDF',
      });
      return;
    }

    if (formData.file.type !== 'application/pdf') {
      setAlert({
        type: 'error',
        message: 'Por favor, selecione um arquivo PDF válido',
      });
      return;
    }

    const maxSizeMB = 10;
    if (formData.file.size > maxSizeMB * 1024 * 1024) {
      setAlert({
        type: 'error',
        message: `O arquivo não pode exceder ${maxSizeMB}MB`,
      });
      return;
    }

    setLoading(true);

    try {
      const formDataToSend = new FormData();
      formDataToSend.append('title', formData.title.trim());
      formDataToSend.append('author', formData.author.trim());
      formDataToSend.append('file', formData.file);

      await articleService.upload(formDataToSend);

      setAlert({
        type: 'success',
        message: 'PDF enviado com sucesso! O texto está sendo extraído e processado...',
      });

      setTimeout(() => navigate('/'), 2000);
    } catch (error) {
      setAlert({
        type: 'error',
        message: error.response?.data?.errors?.[0] || 'Erro ao enviar PDF',
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="upload-container">
      {alert && (
        <Alert
          type={alert.type}
          message={alert.message}
          onClose={() => setAlert(null)}
        />
      )}

      <div className="upload-card">
        <h1>Upload de PDF</h1>
        <p>Envie um PDF para extrair texto e gerar resumo automático</p>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="title">Título</label>
            <input
              type="text"
              id="title"
              name="title"
              value={formData.title}
              onChange={handleChange}
              placeholder="Ex: Artigo sobre IA"
              required
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="author">Autor</label>
            <input
              type="text"
              id="author"
              name="author"
              value={formData.author}
              onChange={handleChange}
              placeholder="Ex: João Silva"
              required
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="file">Arquivo PDF</label>
            <div className="file-input-wrapper">
              <input
                type="file"
                id="file"
                name="file"
                accept=".pdf"
                onChange={handleChange}
                required
                disabled={loading}
              />
              <span className="file-label">
                {formData.file ? formData.file.name : 'Selecione um arquivo PDF'}
              </span>
            </div>
            <small>Máximo 10MB. Apenas arquivos PDF.</small>
          </div>

          <div className="form-actions">
            <button type="submit" disabled={loading}>
              {loading ? 'Enviando...' : 'Enviar PDF'}
            </button>
            <button
              type="button"
              onClick={() => navigate('/')}
              disabled={loading}
              className="btn-cancel"
            >
              Cancelar
            </button>
          </div>
        </form>

        <div className="upload-info">
          <h3>Como funciona?</h3>
          <ol>
            <li>Envie um arquivo PDF com o artigo</li>
            <li>O sistema extrai o texto do documento</li>
            <li>IA gera um resumo automático do conteúdo</li>
            <li>O artigo fica disponível na biblioteca</li>
          </ol>
        </div>
      </div>
    </div>
  );
};

export default UploadPdf;
