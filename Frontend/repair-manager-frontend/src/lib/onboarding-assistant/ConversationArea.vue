<template>
  <div class="conversation-area">
    <div class="messages" ref="messagesContainer">
      <div 
        v-for="(message, index) in messages" 
        :key="index" 
        class="message"
        :class="message.role"
      >
        <div 
          class="message-content" 
          v-html="formatMessage(message.content)"
        ></div>
      </div>
      <div v-if="isLoading" class="message assistant loading">
        <div class="typing-indicator">
          <span></span>
          <span></span>
          <span></span>
        </div>
      </div>
    </div>
    <div class="message-input">
      <input 
        type="text" 
        v-model="inputMessage" 
        @keyup.enter="sendMessage"
        placeholder="Ask a question..."
        :disabled="isLoading"
      />
      <button @click="sendMessage" :disabled="isLoading || !inputMessage.trim()">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <line x1="22" y1="2" x2="11" y2="13"></line>
          <polygon points="22 2 15 22 11 13 2 9 22 2"></polygon>
        </svg>
      </button>
    </div>
  </div>
</template>

<script>
// Import the marked library
import { marked } from 'marked';
import DOMPurify from 'dompurify';

export default {
  name: 'ConversationArea',
  props: {
    messages: {
      type: Array,
      default: () => []
    },
    isLoading: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      inputMessage: '',
      highlightRegex: /\[\[highlight:([^|]+)\|([^\]]+)\]\]/g
    };
  },
  watch: {
    messages() {
      this.$nextTick(() => {
        this.scrollToBottom();
        // Process any highlight instructions in the latest message
        if (this.messages.length > 0 && this.messages[this.messages.length - 1].role === 'assistant') {
          this.processHighlights(this.messages[this.messages.length - 1].content);
        }
      });
    }
  },
  mounted() {
    this.scrollToBottom();
  },
  methods: {
    sendMessage() {
      if (this.inputMessage.trim() && !this.isLoading) {
        this.$emit('send-message', this.inputMessage);
        this.inputMessage = '';
      }
    },
    scrollToBottom() {
      if (this.$refs.messagesContainer) {
        this.$refs.messagesContainer.scrollTop = this.$refs.messagesContainer.scrollHeight;
      }
    },
    formatMessage(message) {
      if (!message) return '';
      
      // Remove OpenAI retrieval citations - handle all formats
      message = message.replace(/【\d+:\d+†[a-zA-Z0-9_.-]+】/g, '');
      message = message.replace(/【\d+:\d+†[a-zA-Z0-9_.-]+】/g, ''); // Handle the actual format from the example
      message = message.replace(/\[\d+:\d+\]/g, '');
      message = message.replace(/\(\d+:\d+\)/g, '');
      
      // Try to parse the new parenthesis-based format with the exact structure from the example
      const userResponseMatch = message.match(/\(USER_RESPONSE\)[\s\n]*([\s\S]*?)(?=\s*\(HIGHLIGHT_ELEMENT\)|$)/);
      const highlightMatch = message.match(/\(HIGHLIGHT_ELEMENT\)\s*\[([\s\S]*?)\]/);
      
      // Debug logging
      console.log('Parsing message format:', { 
        hasUserResponse: !!userResponseMatch, 
        hasHighlight: !!highlightMatch,
        highlightContent: highlightMatch ? highlightMatch[1] : null
      });
      
      if (userResponseMatch) {
        const userResponse = userResponseMatch[1];
        
        // Process highlight element if present
        if (highlightMatch && highlightMatch[1] !== 'none') {
          const highlightLine = highlightMatch[1];
          
          if (highlightLine.startsWith('highlight:')) {
            // Log the highlight data for debugging
            console.log('Processing highlight:', highlightLine);
            
            const [selectorPart, description] = highlightLine.replace('highlight:', '').split('|');
            console.log('Parsed highlight parts:', { selectorPart, description });
            
            const selector = Object.fromEntries(
              selectorPart.split(',').map(kv => kv.split('=').map(x => x.trim()))
            );
            console.log('Selector object:', selector);
            
            // Convert selector object to selector string
            const selectorString = Object.entries(selector)
              .map(([key, value]) => `${key}=${value}`)
              .join(',');
            console.log('Final selector string:', selectorString);
              
            // Emit the highlight event with a slight delay to ensure DOM is ready
            setTimeout(() => {
              this.$emit('highlight-element', { selectorData: selectorString, description });
            }, 500);
          }
        }
        
        // Parse markdown and sanitize
        const html = marked(userResponse);
        return DOMPurify.sanitize(html);
      } else {
        // Try the previous ###SECTION### format
        const parts = message.split(/###([A-Z_]+)###\n?/);
        const sectionMap = {};
        
        for (let i = 1; i < parts.length; i += 2) {
          sectionMap[parts[i]] = parts[i + 1]?.trim();
        }
        
        // If the message follows the structured format
        if (sectionMap.USER_RESPONSE) {
          // Process the highlight element section
          if (sectionMap.HIGHLIGHT_ELEMENT && sectionMap.HIGHLIGHT_ELEMENT !== 'none') {
            const highlightLine = sectionMap.HIGHLIGHT_ELEMENT;
            
            if (highlightLine.startsWith('highlight:')) {
              const [selectorPart, description] = highlightLine.replace('highlight:', '').split('|');
              const selector = Object.fromEntries(
                selectorPart.split(',').map(kv => kv.split('=').map(x => x.trim()))
              );
              
              // Convert selector object to selector string
              const selectorString = Object.entries(selector)
                .map(([key, value]) => `${key}=${value}`)
                .join(',');
                
              this.$emit('highlight-element', { selectorData: selectorString, description });
            }
          }
          
          // Parse markdown and sanitize
          const html = marked(sectionMap.USER_RESPONSE);
          return DOMPurify.sanitize(html);
        } else {
          // Fallback to the old format for backward compatibility
          
          // Process highlight markup and store for later use
          const cleanedMessage = message.replace(this.highlightRegex, (match, selector, description) => {
            return description; // Replace with just the description
          });
          
          // Find the first highlight in the message
          const match = this.highlightRegex.exec(message);
          if (match) {
            // Use direct array indexing instead of destructuring to avoid unused variables
            const selectorData = match[1];
            const description = match[2];
            this.$emit('highlight-element', { selectorData, description });
          }
          
          // Parse markdown and sanitize
          const html = marked(cleanedMessage);
          return DOMPurify.sanitize(html);
        }
      }
    },
    processHighlights(message) {
      if (!message) return;
      
      // Find the first highlight in the message
      const match = this.highlightRegex.exec(message);
      if (match) {
        // Use direct array indexing instead of destructuring to avoid unused variables
        const selectorData = match[1];
        const description = match[2];
        this.$emit('highlight-element', { selectorData, description });
      }
      
      // Reset regex lastIndex for future use
      this.highlightRegex.lastIndex = 0;
    }
  }
};
</script>

<style scoped>
.conversation-area {
  display: flex;
  flex-direction: column;
  height: calc(100% - 60px);
}

.messages {
  flex-grow: 1;
  overflow-y: auto;
  padding: 10px;
  display: flex;
  flex-direction: column;
}

.message {
  margin-bottom: 10px;
  max-width: 80%;
  padding: 8px 12px;
  border-radius: 18px;
  word-break: break-word;
  overflow-wrap: break-word;
  width: fit-content;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
}

.message.user {
  align-self: flex-end;
  background-color: #4a90e2;
  color: white;
  border-bottom-right-radius: 4px;
}

.message.assistant {
  align-self: flex-start;
  background-color: #f1f1f1;
  color: #333;
  border-bottom-left-radius: 4px;
}

.message-input {
  display: flex;
  padding: 10px;
  border-top: 1px solid #e0e0e0;
}

.message-input input {
  flex-grow: 1;
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 20px;
  margin-right: 8px;
}

.message-input button {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background-color: #4a90e2;
  color: white;
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.message-input button:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

.typing-indicator {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 4px 8px;
}

.typing-indicator span {
  height: 8px;
  width: 8px;
  margin: 0 2px;
  background-color: #888;
  border-radius: 50%;
  display: inline-block;
  animation: bounce 1.5s infinite ease-in-out;
}

.typing-indicator span:nth-child(1) {
  animation-delay: 0s;
}

.typing-indicator span:nth-child(2) {
  animation-delay: 0.2s;
}

.typing-indicator span:nth-child(3) {
  animation-delay: 0.4s;
}

@keyframes bounce {
  0%, 60%, 100% {
    transform: translateY(0);
  }
  30% {
    transform: translateY(-5px);
  }
}

/* Markdown styling */
.message-content {
  line-height: 1.5;
  max-width: 100%;
  overflow-wrap: break-word;
  width: 100%;
  box-sizing: border-box;
  display: block;
  padding: 0 10px;
}

.message-content p {
  margin: 0 0 10px 0;
}

.message-content p:last-child {
  margin-bottom: 0;
}

.message-content ul, 
.message-content ol {
  margin: 10px 0;
  padding-left: 30px;
  padding-right: 10px;
  box-sizing: border-box;
  max-width: 100%;
  width: calc(100% - 30px);
  display: block;
}

.message-content li {
  margin-bottom: 5px;
  width: 100%;
  box-sizing: border-box;
  overflow-wrap: break-word;
  word-break: break-word;
}

.message-content strong {
  font-weight: bold;
}

.message-content em {
  font-style: italic;
}

.message-content code {
  font-family: monospace;
  background-color: rgba(0, 0, 0, 0.05);
  padding: 2px 4px;
  border-radius: 3px;
}

.message-content pre {
  background-color: rgba(0, 0, 0, 0.05);
  padding: 10px;
  border-radius: 5px;
  overflow-x: auto;
  margin: 10px 0;
}

.message-content pre code {
  background-color: transparent;
  padding: 0;
}
</style>
