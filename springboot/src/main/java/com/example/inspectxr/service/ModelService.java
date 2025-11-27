package com.example.inspectxr.service;

import com.example.inspectxr.dtos.ModelRequestDTO;
import com.example.inspectxr.dtos.ModelResponseDTO;
import com.example.inspectxr.entity.ModelEntity;
import com.example.inspectxr.entity.UserEntity;
import com.example.inspectxr.mapper.ModelMapper;
import com.example.inspectxr.repository.ModelRepository;
import com.example.inspectxr.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.web.multipart.MultipartFile;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;
import java.util.stream.Collectors;

@Service
public class ModelService {

    private final ModelRepository modelRepository;
    private final UserRepository userRepository;
    private final ModelMapper modelMapper;

    @Autowired
    public ModelService(ModelRepository modelRepository, UserRepository userRepository, ModelMapper modelMapper) {
        this.modelRepository = modelRepository;
        this.userRepository = userRepository;
        this.modelMapper = modelMapper;
    }

    public ModelResponseDTO getModelById(Long id) {
        ModelEntity model = modelRepository.findById(id)
                .orElseThrow(() ->
                        new ResponseStatusException(HttpStatus.NOT_FOUND, "Model not found"));
        return modelMapper.toDTO(model);
    }

    public ModelResponseDTO addModel(ModelRequestDTO dto, MultipartFile file) {
        UserEntity owner = userRepository.findById(dto.getOwnerId())
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "User not found"));
        ModelEntity model = modelMapper.toEntity(dto);
        model.setOwner(owner);
        model.setObjectUrl(dto.getObjectUrl());
        return modelMapper.toDTO(modelRepository.save(model));
    }

    public void deleteModel(Long id) {
        if (!modelRepository.existsById(id)) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Model not found");
        }
        modelRepository.deleteById(id);
    }

    public List<ModelResponseDTO> getModelsByOwnerId(Long ownerId) {
        if (!userRepository.existsById(ownerId)) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "User not found");
        }

        List<ModelEntity> models = modelRepository.findByOwnerId(ownerId);
        return models.stream()
                .map(modelMapper::toDTO)
                .collect(Collectors.toList());
    }

}

