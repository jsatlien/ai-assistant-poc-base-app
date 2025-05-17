<template>
  <div 
    class="assistant-widget" 
    :class="{ 
      'expanded': isExpanded, 
      'minimized': isMinimized,
      'position-bottom-right': position === 'bottom-right',
      'position-bottom-left': position === 'bottom-left',
      'position-top-right': position === 'top-right',
      'position-top-left': position === 'top-left',
      'position-panel-right': position === 'panel-right',
      'position-panel-left': position === 'panel-left'
    }"
    ref="widgetRef"
    @click="handleWidgetClick"
  >
    <div class="assistant-header" @mousedown="startDrag">
      <div class="assistant-avatar">
        <img src="./assistant-avatar.svg" alt="Assistant" />
      </div>
      <div v-if="isExpanded" class="assistant-title">{{ assistantName }}</div>
      <div class="assistant-controls">
        <button @click="toggleSettings" class="settings-button" v-if="isExpanded">
          <span class="gear-icon">⚙️</span>
        </button>
        <button @click="toggleMinimize" class="minimize-button" v-if="isExpanded">
          <span>−</span>
        </button>
        <button @click="toggleExpand" class="toggle-button">
          {{ isExpanded ? '×' : '+' }}
        </button>
      </div>
      <div v-if="showSettings" class="settings-dropdown">
        <div class="settings-title">Position</div>
        <div class="settings-option" @click="setPosition('bottom-right')">Bottom Right</div>
        <div class="settings-option" @click="setPosition('bottom-left')">Bottom Left</div>
        <div class="settings-option" @click="setPosition('top-right')">Top Right</div>
        <div class="settings-option" @click="setPosition('top-left')">Top Left</div>
      </div>
    </div>
    
    <ConversationArea 
      v-if="isExpanded" 
      :messages="messages" 
      :isLoading="isLoading"
      @send-message="handleSendMessage"
      @highlight-element="handleHighlightElement"
    />
  </div>
</template>

<script>
import ConversationArea from './ConversationArea.vue';
import axios from 'axios';

export default {
  name: 'AssistantWidget',
  components: {
    ConversationArea
  },
  props: {
    apiBaseUrl: {
      type: String,
      default: 'http://localhost:5000/api'
    },
    assistantName: {
      type: String,
      default: 'Onboarding Assistant'
    },
    applicationName: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      isExpanded: false,
      isMinimized: false,
      showSettings: false,
      position: localStorage.getItem('onboarding_assistant_position') || 'bottom-right',
      isDragging: false,
      dragOffset: { x: 0, y: 0 },
      messages: [],
      isLoading: false,
      currentRoute: window.location.pathname,
      threadId: sessionStorage.getItem('onboarding_assistant_threadId') || null
    };
  },
  mounted() {
    // Get current route
    this.currentRoute = this.$route ? this.$route.path : window.location.pathname;
    console.log('Assistant widget mounted, current route:', this.currentRoute);
    
    // Watch for route changes if Vue Router is available
    if (this.$router) {
      this.$router.afterEach((to) => {
        this.currentRoute = to.path;
        console.log('Route changed to:', this.currentRoute);
      });
    }
    
    // Load conversation history from sessionStorage
    this.loadConversationHistory();
    
    // Add event listener to close settings dropdown when clicking outside
    document.addEventListener('click', this.closeSettingsOnClickOutside);
    
    // Apply panel positioning if needed
    this.applyPanelPositioning();
  },
  
  beforeDestroy() {
    // Remove event listeners
    document.removeEventListener('click', this.closeSettingsOnClickOutside);
  },
  methods: {
    handleWidgetClick(event) {
      // Don't handle clicks on control buttons - they have their own handlers
      if (event.target.closest('.toggle-button') || 
          event.target.closest('.minimize-button') || 
          event.target.closest('.settings-button') || 
          event.target.closest('.settings-dropdown')) {
        return;
      }
      
      console.log('Widget clicked, current states - expanded:', this.isExpanded, 'minimized:', this.isMinimized);
      
      // If widget is minimized, restore it
      if (this.isMinimized) {
        console.log('Widget is minimized, restoring');
        this.isMinimized = false;
        this.applyPanelPositioning();
        return;
      }
      
      // If widget is not expanded, expand it
      if (!this.isExpanded) {
        console.log('Widget is collapsed, expanding');
        this.isExpanded = true;
        this.isMinimized = false;
        this.applyPanelPositioning();
      }
    },
    toggleExpand(event) {
      console.log('Toggle expand clicked, current states - expanded:', this.isExpanded, 'minimized:', this.isMinimized);
      
      // If minimized, restore first
      if (this.isMinimized) {
        this.isMinimized = false;
      }
      
      // Toggle expanded state
      this.isExpanded = !this.isExpanded;
      
      // If collapsing, ensure settings are closed
      if (!this.isExpanded) {
        this.showSettings = false;
      }
      
      console.log('New states - expanded:', this.isExpanded, 'minimized:', this.isMinimized);
      this.applyPanelPositioning();
      
      // Stop event propagation to prevent handleWidgetClick from being triggered
      if (event) {
        event.stopPropagation();
      }
    },
    toggleMinimize(event) {
      console.log('Toggle minimize clicked');
      this.isMinimized = !this.isMinimized;
      
      // Apply or remove panel positioning based on minimized state
      this.applyPanelPositioning();
      
      // Don't close settings when minimizing
      event.stopPropagation();
    },
    toggleSettings(event) {
      console.log('Toggle settings clicked');
      this.showSettings = !this.showSettings;
      event.stopPropagation();
    },
    closeSettingsOnClickOutside(event) {
      if (this.showSettings && !event.target.closest('.settings-button') && !event.target.closest('.settings-dropdown')) {
        this.showSettings = false;
      }
    },
    setPosition(position) {
      console.log('Setting position to:', position);
      this.position = position;
      localStorage.setItem('onboarding_assistant_position', position);
      this.showSettings = false;
      this.applyPanelPositioning();
    },
    applyPanelPositioning() {
      // This method is kept for compatibility but no longer applies panel positioning
      // It may be used in the future if panel positioning is re-implemented
      console.log('Panel positioning disabled');
    },

    setDefaultGreeting() {
      let greetingMessage;
      
      if (this.applicationName) {
        greetingMessage = `Hello, I'm ${this.assistantName}, your onboarding assistant for ${this.applicationName}! I can walk you through general operations, guide you through workflows, or answer any questions about what you're seeing. I'm here to help!`;
      } else {
        greetingMessage = `Hello! I'm your onboarding assistant! How can I help you today?`;
      }
      
      this.messages = [
        {
          role: 'assistant',
          content: greetingMessage
        }
      ];
    },
    saveConversationHistory() {
      try {
        sessionStorage.setItem('onboarding_assistant_history', JSON.stringify(this.messages));
      } catch (error) {
        console.error('Error saving conversation history:', error);
      }
    },
    loadConversationHistory() {
      try {
        // Load conversation history
        const savedHistory = sessionStorage.getItem('onboarding_assistant_history');
        if (savedHistory) {
          this.messages = JSON.parse(savedHistory);
          console.log('Loaded conversation history:', this.messages.length, 'messages');
        } else {
          // Set default greeting if no history exists
          this.setDefaultGreeting();
        }
        
        // Thread ID is already loaded in data() from sessionStorage
        if (this.threadId) {
          console.log('Loaded existing thread ID:', this.threadId);
        }
      } catch (error) {
        console.error('Error loading conversation history:', error);
        this.setDefaultGreeting();
      }
    },
    // Simplified drag handling since we're using fixed positioning
    startDrag(event) {
      // Only allow dragging when expanded and not clicking on the toggle button
      if (event.target.closest('.toggle-button') || !this.isExpanded) return;
      console.log('Drag handling is disabled with fixed positioning');
    },
    async handleSendMessage(message) {
      // Add user message to conversation
      this.messages.push({
        role: 'user',
        content: message
      });
      
      // Save conversation history after adding user message
      this.saveConversationHistory();
      
      // Set loading state
      this.isLoading = true;
      
      try {
        // Send message to backend API with thread ID if available
        const response = await axios.post(`${this.apiBaseUrl}/query`, {
          query: message,
          route: this.currentRoute,
          threadId: this.threadId
        });
        
        console.log('Response from backend:', response.data);
        
        // Save the thread ID for future requests
        if (response.data.threadId || response.data.ThreadId) {
          this.threadId = response.data.threadId || response.data.ThreadId;
          sessionStorage.setItem('onboarding_assistant_threadId', this.threadId);
          console.log('Thread ID saved:', this.threadId);
        }
        
        // Add assistant response to conversation
        this.messages.push({
          role: 'assistant',
          content: response.data.message || response.data.Message || "I'm sorry, I couldn't process your request."
        });
        
        // Save conversation history after adding assistant response
        this.saveConversationHistory();
        
        // Handle any actions returned by the assistant
        const actions = response.data.actions || response.data.Actions || [];
        if (actions.length > 0) {
          actions.forEach(action => {
            const actionType = action.Type || action.type;
            const elementId = action.ElementId || action.elementId;
            
            if (actionType === 'highlight' && elementId) {
              this.highlightElement(elementId);
            }
          });
        }
      } catch (error) {
        console.error('Error querying assistant:', error);
        
        // Add error message to conversation
        this.messages.push({
          role: 'assistant',
          content: "I'm sorry, I encountered an error while processing your request. Please try again later."
        });
        
        // Save conversation history after adding error message
        this.saveConversationHistory();
      } finally {
        // Clear loading state
        this.isLoading = false;
      }
    },
    findElementBySelectorData(selectorString) {
      try {
        // Parse the selector data into parts
        const parts = Object.fromEntries(
          selectorString.split(',').map(p => {
            const [key, value] = p.split('=').map(x => x.trim());
            return [key, value];
          })
        );

        const tag = parts.tag || '*';
        const text = parts.text;
        const id = parts.id;
        const className = parts.class;

        // Try to find by ID first (most specific)
        if (id) {
          const element = document.getElementById(id);
          if (element) return element;
        }

        // Try to find by class
        if (className) {
          const elements = document.getElementsByClassName(className);
          if (elements.length > 0) {
            // If we also have text, filter by text content
            if (text) {
              for (const el of elements) {
                if (el.textContent?.trim() === text) {
                  return el;
                }
              }
            } else {
              return elements[0]; // Return first match if no text specified
            }
          }
        }

        // Find by tag and text content
        if (text) {
          const candidates = Array.from(document.querySelectorAll(tag));
          return candidates.find(el => el.textContent?.trim() === text);
        }

        // If all else fails, try as a CSS selector
        return document.querySelector(selectorString);
      } catch (error) {
        console.error('Error parsing selector data:', error);
        return null;
      }
    },
    
    highlightElement(element, description) {
      if (!element) {
        console.warn('Element not found for highlighting');
        return;
      }
      
      // Initialize Shepherd if not already done
      if (!window.Shepherd) {
        console.warn('Shepherd.js not found. Element highlighting will use fallback method.');
        // Fallback highlighting method
        const originalBackground = element.style.backgroundColor;
        const originalTransition = element.style.transition;
        
        element.style.transition = 'background-color 0.3s ease';
        element.style.backgroundColor = 'rgba(74, 144, 226, 0.3)';
        element.scrollIntoView({ behavior: 'smooth', block: 'center' });
        
        setTimeout(() => {
          element.style.backgroundColor = originalBackground;
          element.style.transition = originalTransition;
        }, 3000);
        return;
      }
      
      // Use Shepherd.js for highlighting
      const tour = new window.Shepherd.Tour({
        defaultStepOptions: {
          cancelIcon: {
            enabled: true
          },
          classes: 'assistant-highlight-tooltip',
          scrollTo: true
        }
      });
      
      tour.addStep({
        id: 'highlight-step',
        attachTo: {
          element: element,
          on: 'bottom'
        },
        text: description || 'This element is relevant to your query',
        buttons: [
          {
            text: 'Got it',
            action: tour.complete
          }
        ]
      });
      
      tour.start();
    },
    
    handleHighlightElement({ selectorData, description }) {
      console.log('Highlighting element request received:', { selectorData, description });
      
      if (!selectorData) {
        console.warn('No selector data provided for highlighting');
        return;
      }
      
      // Try to find the element
      const element = this.findElementBySelectorData(selectorData);
      
      if (element) {
        console.log('Element found:', element);
        this.highlightElement(element, description);
      } else {
        console.warn(`Element not found for selector data: ${selectorData}`);
        // Try again after a short delay in case DOM is still updating
        setTimeout(() => {
          const retryElement = this.findElementBySelectorData(selectorData);
          if (retryElement) {
            console.log('Element found on retry:', retryElement);
            this.highlightElement(retryElement, description);
          } else {
            // Just log to console but don't show any user-facing message
            console.warn(`Element still not found after retry for: ${selectorData}`);
          }
        }, 1000);
      }
    }
  }
};
</script>

<style scoped>
.assistant-widget {
  position: fixed;
  z-index: 9999;
  width: 60px;
  height: 60px;
  background-color: #4a90e2;
  border-radius: 30px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
  transition: all 0.3s ease-in-out;
  transform-origin: bottom center;
  overflow: hidden;
  cursor: pointer;
  animation: pulse 2s infinite;
}

/* Default position (bottom-right) */
.assistant-widget.position-bottom-right {
  bottom: 20px;
  right: 20px;
}

.assistant-widget.position-bottom-left {
  bottom: 20px;
  left: 20px;
}

.assistant-widget.position-top-right {
  top: 20px;
  right: 20px;
}

.assistant-widget.position-top-left {
  top: 20px;
  left: 20px;
}

/* Panel positioning removed */

@keyframes pulse {
  0% {
    box-shadow: 0 0 0 0 rgba(74, 144, 226, 0.7);
  }
  70% {
    box-shadow: 0 0 0 10px rgba(74, 144, 226, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(74, 144, 226, 0);
  }
}

.assistant-widget.expanded {
  width: 350px;
  height: 500px;
  border-radius: 10px;
  animation: none;
}

.assistant-widget.expanded.position-panel-right,
.assistant-widget.expanded.position-panel-left {
  width: 320px;
  height: 100%;
  border-radius: 0;
}

.assistant-widget.minimized {
  width: 60px !important;
  height: 60px !important;
  border-radius: 30px !important;
}

.assistant-widget.minimized .conversation-area,
.assistant-widget.minimized .assistant-title {
  display: none;
}

/* Minimized panel positioning removed */

.assistant-header {
  display: flex;
  align-items: center;
  padding: 10px;
  cursor: move;
  background-color: #4a90e2;
  color: white;
}

.assistant-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background-color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 10px;
}

.assistant-avatar img {
  width: 30px;
  height: 30px;
}

.assistant-title {
  flex-grow: 1;
  font-weight: bold;
}

.assistant-controls {
  display: flex;
}

.toggle-button,
.minimize-button,
.settings-button {
  background: none;
  border: none;
  color: white;
  font-size: 20px;
  cursor: pointer;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  margin-left: 5px;
}

.toggle-button:hover,
.minimize-button:hover,
.settings-button:hover {
  background-color: rgba(255, 255, 255, 0.2);
}

.settings-dropdown {
  position: absolute;
  top: 50px;
  right: 10px;
  background-color: white;
  border-radius: 5px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
  padding: 10px;
  z-index: 10000;
  width: 150px;
}

.settings-title {
  font-weight: bold;
  margin-bottom: 8px;
  color: #333;
  font-size: 14px;
  border-bottom: 1px solid #eee;
  padding-bottom: 5px;
}

.settings-option {
  padding: 8px 5px;
  cursor: pointer;
  color: #333;
  font-size: 13px;
  border-radius: 4px;
}

.settings-option:hover {
  background-color: #f5f5f5;
  color: #4a90e2;
}
</style>
