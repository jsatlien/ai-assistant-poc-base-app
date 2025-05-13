<template>
  <div class="product-categories-page">
    <h1>Product Categories</h1>
    <div class="page-actions">
      <button class="btn-primary" @click="openCategoryModal()">Add Category</button>
      <div class="search-container">
        <input type="text" v-model="searchQuery" placeholder="Search categories..." class="search-input" />
      </div>
    </div>

    <div v-if="filteredCategories.length > 0" class="categories-grid">
      <div v-for="category in filteredCategories" :key="category.id" class="category-card">
        <div class="category-header">
          <h3>{{ category.name }}</h3>
        </div>
        <p class="category-description">{{ category.description }}</p>
        <div class="category-actions">
          <button class="btn-edit" @click="editCategory(category)">Edit</button>
          <button class="btn-delete" @click="confirmDeleteCategory(category)">Delete</button>
        </div>
      </div>
    </div>

    <div v-else class="empty-state">
      <p>No product categories found. Add your first category to get started.</p>
    </div>

    <!-- Category Modal -->
    <div v-if="showCategoryModal" class="modal">
      <div class="modal-content">
        <div class="modal-header">
          <h2>{{ isEditing ? 'Edit Category' : 'Add Category' }}</h2>
          <button class="close-btn" @click="closeCategoryModal">&times;</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="saveCategory">
            <div class="form-group">
              <label for="categoryName">Name</label>
              <input 
                id="categoryName" 
                v-model="currentCategory.name" 
                class="form-control"
                required
              />
            </div>
            <div class="form-group">
              <label for="categoryDescription">Description</label>
              <textarea 
                id="categoryDescription" 
                v-model="currentCategory.description" 
                class="form-control"
                rows="3"
              ></textarea>
            </div>
            <div class="form-actions">
              <button type="button" class="btn-secondary" @click="closeCategoryModal">Cancel</button>
              <button type="submit" class="btn-primary">Save</button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Confirmation Modal -->
    <div v-if="showConfirmationModal" class="modal">
      <div class="modal-content confirmation-modal">
        <div class="modal-header">
          <h2>Confirm Delete</h2>
          <button class="close-btn" @click="closeConfirmationModal">&times;</button>
        </div>
        <div class="modal-body">
          <p>Are you sure you want to delete the category "{{ categoryToDelete?.name }}"?</p>
          <div class="form-actions">
            <button class="btn-secondary" @click="closeConfirmationModal">Cancel</button>
            <button class="btn-delete" @click="deleteCategory">Delete</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ProductCategories',
  data() {
    return {
      searchQuery: '',
      showCategoryModal: false,
      showConfirmationModal: false,
      isEditing: false,
      currentCategory: {
        id: null,
        name: '',
        description: ''
      },
      categoryToDelete: null
    }
  },
  computed: {
    productCategories() {
      return this.$store.getters.getProductCategories
    },
    filteredCategories() {
      if (!this.searchQuery) return this.productCategories
      
      const query = this.searchQuery.toLowerCase()
      return this.productCategories.filter(category => 
        category.name.toLowerCase().includes(query) || 
        (category.description && category.description.toLowerCase().includes(query))
      )
    }
  },
  created() {
    this.$store.dispatch('fetchProductCategories')
  },
  methods: {
    openCategoryModal(category = null) {
      this.isEditing = !!category
      this.currentCategory = category 
        ? { ...category } 
        : { id: null, name: '', description: '' }
      this.showCategoryModal = true
    },
    closeCategoryModal() {
      this.showCategoryModal = false
      this.currentCategory = { id: null, name: '', description: '' }
    },
    async saveCategory() {
      if (!this.currentCategory.name.trim()) {
        alert('Category name is required')
        return
      }

      try {
        if (this.isEditing) {
          await this.$store.dispatch('updateProductCategory', this.currentCategory)
        } else {
          await this.$store.dispatch('addProductCategory', this.currentCategory)
        }
        this.closeCategoryModal()
      } catch (error) {
        console.error('Error saving category:', error)
        alert('Failed to save category. Please try again.')
      }
    },
    editCategory(category) {
      this.openCategoryModal(category)
    },
    confirmDeleteCategory(category) {
      this.categoryToDelete = category
      this.showConfirmationModal = true
    },
    closeConfirmationModal() {
      this.showConfirmationModal = false
      this.categoryToDelete = null
    },
    async deleteCategory() {
      if (!this.categoryToDelete) return

      try {
        await this.$store.dispatch('deleteProductCategory', this.categoryToDelete.id)
        this.closeConfirmationModal()
      } catch (error) {
        console.error('Error deleting category:', error)
        alert('Failed to delete category. Please try again.')
      }
    }
  }
}
</script>

<style scoped>
.product-categories-page {
  padding: 2rem;
}

h1 {
  margin-bottom: 1.5rem;
  color: #333;
  font-weight: 600;
}

.page-actions {
  display: flex;
  justify-content: space-between;
  margin-bottom: 2rem;
}

.search-container {
  flex: 0 0 300px;
}

.search-input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 1rem;
}

.categories-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.category-card {
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.category-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.category-header {
  margin-bottom: 1rem;
}

.category-header h3 {
  margin: 0;
  color: #333;
  font-weight: 600;
  font-size: 1.25rem;
}

.category-description {
  color: #666;
  margin-bottom: 1.5rem;
  flex-grow: 1;
}

.category-actions {
  display: flex;
  gap: 0.5rem;
  margin-top: auto;
}

.empty-state {
  background-color: #f9f9f9;
  border-radius: 8px;
  padding: 3rem;
  text-align: center;
  color: #666;
}

/* Button Styles */
.btn-primary {
  background-color: #08c;
  color: #fff;
  border: none;
  border-radius: 8px;
  padding: 0.75rem 1.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.btn-primary:hover {
  background-color: #0077b3;
}

.btn-secondary {
  background-color: #f5f5f5;
  color: #333;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 0.75rem 1.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-secondary:hover {
  background-color: #e5e5e5;
}

.btn-edit {
  background-color: transparent;
  color: #08c;
  border: 1px solid #08c;
  border-radius: 4px;
  padding: 0.4rem 0.7rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 0.85rem;
}

.btn-edit:hover {
  background-color: #08c;
  color: #fff;
}

.btn-delete {
  background-color: transparent;
  color: #dc3545;
  border: 1px solid #dc3545;
  border-radius: 4px;
  padding: 0.4rem 0.7rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 0.85rem;
}

.btn-delete:hover {
  background-color: #dc3545;
  color: #fff;
}

/* Modal Styles */
.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background-color: #fff;
  border-radius: 8px;
  width: 500px;
  max-width: 90%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
}

.confirmation-modal {
  width: 400px;
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid #eee;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h2 {
  margin: 0;
  font-size: 1.5rem;
  color: #333;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #999;
}

.close-btn:hover {
  color: #333;
}

.modal-body {
  padding: 1.5rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #555;
}

.form-control {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

.form-control:focus {
  border-color: #08c;
  outline: none;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1.5rem;
}
</style>
