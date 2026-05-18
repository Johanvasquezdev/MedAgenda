import api from '@/lib/api';
import { ChatRequest, ChatResponse } from '@/types/api.types';

export class ChatbotService {
  static async enviarMensaje(request: ChatRequest): Promise<ChatResponse> {
    const response = await api.post<ChatResponse>('/api/chatbot/mensaje', request);
    return response.data;
  }
}