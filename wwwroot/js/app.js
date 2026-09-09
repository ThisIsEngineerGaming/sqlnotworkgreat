const API_URL = '/api/Films';

const filmsBody = document.getElementById('filmsBody');
const form = document.getElementById('filmForm');
const filmIdInput = document.getElementById('filmId');
const titleInput = document.getElementById('title');
const directorInput = document.getElementById('director');
const releaseYearInput = document.getElementById('releaseYear');
const genreInput = document.getElementById('genre');
const ratingInput = document.getElementById('rating');
const photoFileInput = document.getElementById('photoFile');
const formTitle = document.getElementById('formTitle');
const submitBtn = document.getElementById('submitBtn');
const cancelBtn = document.getElementById('cancelBtn');
const errorBox = document.getElementById('errorBox');

async function loadFilms() {
  errorBox.textContent = '';
  const res = await fetch(API_URL);
  const films = await res.json();
  renderFilms(films);
}

function renderFilms(films) {
  filmsBody.innerHTML = '';
  for (const film of films) {
    const tr = document.createElement('tr');

    const photoCell = document.createElement('td');
    if (film.photoUrl) {
      const img = document.createElement('img');
      img.src = film.photoUrl;
      photoCell.appendChild(img);
    }

    const editBtn = document.createElement('button');
    editBtn.textContent = 'Edit';
    editBtn.type = 'button';
    editBtn.addEventListener('click', () => startEdit(film));

    const deleteBtn = document.createElement('button');
    deleteBtn.textContent = 'Delete';
    deleteBtn.type = 'button';
    deleteBtn.addEventListener('click', () => deleteFilm(film.id));

    const actionsCell = document.createElement('td');
    actionsCell.appendChild(editBtn);
    actionsCell.appendChild(deleteBtn);

    tr.appendChild(photoCell);
    tr.appendChild(makeCell(film.title));
    tr.appendChild(makeCell(film.director));
    tr.appendChild(makeCell(film.releaseYear));
    tr.appendChild(makeCell(film.genre));
    tr.appendChild(makeCell(film.rating));
    tr.appendChild(actionsCell);

    filmsBody.appendChild(tr);
  }
}

function makeCell(text) {
  const td = document.createElement('td');
  td.textContent = text;
  return td;
}

function startEdit(film) {
  filmIdInput.value = film.id;
  titleInput.value = film.title;
  directorInput.value = film.director;
  releaseYearInput.value = film.releaseYear;
  genreInput.value = film.genre;
  ratingInput.value = film.rating;
  photoFileInput.value = '';

  formTitle.textContent = 'Edit film';
  submitBtn.textContent = 'Save';
  cancelBtn.style.display = 'inline-block';
}

function resetForm() {
  form.reset();
  filmIdInput.value = '';
  formTitle.textContent = 'Add film';
  submitBtn.textContent = 'Add';
  cancelBtn.style.display = 'none';
}

cancelBtn.addEventListener('click', resetForm);

form.addEventListener('submit', async (e) => {
  e.preventDefault();
  errorBox.textContent = '';

  const id = filmIdInput.value;
  const formData = new FormData();
  formData.append('Id', id || '0');
  formData.append('Title', titleInput.value);
  formData.append('Director', directorInput.value);
  formData.append('ReleaseYear', releaseYearInput.value);
  formData.append('Genre', genreInput.value);
  formData.append('Rating', ratingInput.value);
  if (photoFileInput.files[0]) {
    formData.append('photoFile', photoFileInput.files[0]);
  }

  const isEdit = !!id;
  const url = isEdit ? `${API_URL}/${id}` : API_URL;
  const method = isEdit ? 'PUT' : 'POST';

  const res = await fetch(url, { method, body: formData });

  if (!res.ok) {
    const problem = await res.json().catch(() => null);
    errorBox.textContent = problem ? JSON.stringify(problem.errors || problem) : `Request failed: ${res.status}`;
    return;
  }

  resetForm();
  loadFilms();
});

async function deleteFilm(id) {
  if (!confirm('Delete this film?')) return;
  await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
  loadFilms();
}

loadFilms();
