package com.example.inspectxr.service;

import com.example.inspectxr.dtos.ModelResponseDTO;
import com.example.inspectxr.dtos.SessionRequestDTO;
import com.example.inspectxr.dtos.SessionResponseDTO;
import com.example.inspectxr.entity.*;
import com.example.inspectxr.repository.*;
import com.example.inspectxr.mapper.SessionMapper;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ResponseStatusException;
import org.springframework.http.HttpStatus;

import java.time.LocalDateTime;
import java.util.List;
import java.util.stream.Collectors;

@Service
public class SessionService {

    private final SessionRepository sessionRepository;
    private final UserRepository userRepository;
    private final ModelRepository modelRepository;
    private final SessionMapper sessionMapper;

    @Autowired
    public SessionService(SessionRepository sessionRepository, UserRepository userRepository,
                          ModelRepository modelRepository, SessionMapper sessionMapper) {
        this.sessionRepository = sessionRepository;
        this.userRepository = userRepository;
        this.modelRepository = modelRepository;
        this.sessionMapper = sessionMapper;
    }

    public SessionResponseDTO createSession(SessionRequestDTO dto) {
        UserEntity owner = userRepository.findById(dto.getOwnerId())
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Owner not found"));
        ModelEntity model = modelRepository.findById(dto.getModelId())
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Model not found"));

        List<UserEntity> participants = dto.getParticipantIds() != null
                ? userRepository.findAllById(dto.getParticipantIds())
                : List.of();

        SessionEntity session = new SessionEntity();
        session.setOwner(owner);
        session.setModel(model);
        session.setCreatedAt(LocalDateTime.now());
        session.setParticipants(participants);

        return sessionMapper.toDTO(sessionRepository.save(session));
    }

    public SessionResponseDTO getSession(Long id) {
        return sessionRepository.findByIdWithAssociations(id)
                .map(sessionMapper::toDTO)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Session not found"));
    }

    public List<SessionResponseDTO> getAllSessions() {
        return sessionRepository.findAllWithAssociations().stream()
                .map(sessionMapper::toDTO)
                .collect(Collectors.toList());
    }

    public List<SessionResponseDTO> getSessionsByOwnerId(Long ownerId) {
        if (!userRepository.existsById(ownerId)) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "User not found");
        }

        List<SessionEntity> sessions = sessionRepository.findByOwnerIdWithAssociations(ownerId);
        return sessions.stream()
                .map(sessionMapper::toDTO)
                .collect(Collectors.toList());
    }
}
